using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Core.DomainModel.General;
using TaxiCameBack.Core.Utilities;
using TaxiCameBack.Website.Application.StorageProviders;

namespace TaxiCameBack.Website.Application
{
    public static class AppHelpers
    {

        #region Application

        /// <summary>
        /// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>True if the request targets a static resource file.</returns>
        /// <remarks>
        /// These are the file extensions considered to be static resources:
        /// .css
        ///	.gif
        /// .png 
        /// .jpg
        /// .jpeg
        /// .js
        /// .axd
        /// .ashx
        /// </remarks>
        public static bool IsStaticResource(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            string path = request.Path;
            string extension = VirtualPathUtility.GetExtension(path);

            if (extension == null) return false;

            switch (extension.ToLower())
            {
                case ".axd":
                case ".ashx":
                case ".bmp":
                case ".css":
                case ".gif":
                case ".htm":
                case ".html":
                case ".ico":
                case ".jpeg":
                case ".jpg":
                case ".js":
                case ".png":
                case ".rar":
                case ".zip":
                    return true;
            }

            return false;
        }

        #endregion
        
        #region Files

        public static bool FileIsImage(string file)
        {
            var imageFileTypes = new List<string>
            {
                ".jpg", ".jpeg",".gif",".bmp",".png"
            };
            return imageFileTypes.Any(file.Contains);
        }

        public static Image GetImageFromExternalUrl(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            using (var httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var stream = httpWebReponse.GetResponseStream())
                {
                    if (stream != null) return Image.FromStream(stream);
                }
            }
            return null;
        }

        public static string MemberImage(Guid userId, string avatar, string email, int size)
        {
            if (!string.IsNullOrEmpty(avatar))
            {
                // Has an avatar image
                var storageProvider = StorageProvider.Current;
                return storageProvider.BuildFileUrl(userId, "/", avatar, string.Format("?width={0}&crop=0,0,{0},{0}", size));
            }

            return StringUtils.GetGravatarImage(email, size);
        }

        public static string CategoryImage(string image, Guid categoryId, int size)
        {
            var sizeFormat = string.Format("?width={0}&crop=0,0,{0},{0}", size);
            if (!string.IsNullOrEmpty(image))
            {
                var storageProvider = StorageProvider.Current;
                return storageProvider.BuildFileUrl(categoryId, "/", image, sizeFormat);
            }
            //TODO - Return default image for category
            return null;
        }

        public static UploadFileResult UploadFile(HttpPostedFileBase file, string uploadFolderPath, bool onlyImages = false)
        {
            var upResult = new UploadFileResult { UploadSuccessful = true };
            const string imageExtensions = "jpg,jpeg,png,gif";
            var fileName = Path.GetFileName(file.FileName);
            var storageProvider = StorageProvider.Current;

            if (fileName != null)
            {
                // Lower case
                fileName = fileName.ToLower();

                // Get the file extension
                var fileExtension = Path.GetExtension(fileName);

                //Before we do anything, check file size
                if (file.ContentLength > Convert.ToInt32(AppConstants.FileUploadMaximumFileSizeInBytes))
                {
                    //File is too big
                    upResult.UploadSuccessful = false;
                    upResult.ErrorMessage = "File upload is too big";
                    return upResult;
                }

                // now check allowed extensions
                var allowedFileExtensions = AppConstants.FileUploadAllowedExtensions;

                if (onlyImages)
                {
                    allowedFileExtensions = imageExtensions;
                }

                if (!string.IsNullOrEmpty(allowedFileExtensions))
                {
                    // Turn into a list and strip unwanted commas as we don't trust users!
                    var allowedFileExtensionsList = allowedFileExtensions.ToLower().Trim()
                                                     .TrimStart(',').TrimEnd(',').Split(',').ToList();

                    // If can't work out extension then just error
                    if (string.IsNullOrEmpty(fileExtension))
                    {
                        upResult.UploadSuccessful = false;
                        upResult.ErrorMessage = "Sorry an error occured";
                        return upResult;
                    }

                    // Remove the dot then check against the extensions in the web.config settings
                    fileExtension = fileExtension.TrimStart('.');
                    if (!allowedFileExtensionsList.Contains(fileExtension))
                    {
                        upResult.UploadSuccessful = false;
                        upResult.ErrorMessage = "File extension not allowed";
                        return upResult;
                    }
                }

                // Store these here as we may change the values within the image manipulation
                var newFileName = string.Empty;
                var path = string.Empty;

                if (imageExtensions.Split(',').ToList().Contains(fileExtension))
                {
                    // Rotate image if wrong want around
                    using (var sourceimage = Image.FromStream(file.InputStream))
                    {
                        if (sourceimage.PropertyIdList.Contains(0x0112))
                        {
                            int rotationValue = sourceimage.GetPropertyItem(0x0112).Value[0];
                            switch (rotationValue)
                            {
                                case 1: // landscape, do nothing
                                    break;

                                case 8: // rotated 90 right
                                    // de-rotate:
                                    sourceimage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                    break;

                                case 3: // bottoms up
                                    sourceimage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    break;

                                case 6: // rotated 90 left
                                    sourceimage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                    break;
                            }
                        }

                        using (var stream = new MemoryStream())
                        {
                            // Save the image as a Jpeg only
                            sourceimage.Save(stream, ImageFormat.Jpeg);
                            stream.Position = 0;

                            // Change the extension to jpg as that's what we are saving it as
                            fileName = fileName.Replace(fileExtension, "");
                            fileName = string.Concat(fileName, "jpg");
                            file = new MemoryFile(stream, "image/jpeg", fileName);

                            // Sort the file name
                            newFileName = CreateNewFileName(fileName);

                            // Get the storage provider and save file
                            upResult.UploadedFileUrl = storageProvider.SaveAs(uploadFolderPath, newFileName, file);
                        }
                    }
                }
                else
                {
                    // Sort the file name
                    newFileName = CreateNewFileName(fileName);
                    upResult.UploadedFileUrl = storageProvider.SaveAs(uploadFolderPath, newFileName, file);
                }

                upResult.UploadedFileName = newFileName;
            }

            return upResult;
        }

        private static string CreateNewFileName(string fileName)
        {
            return $"{GuidComb.GenerateComb()}_{fileName.Trim(' ').Replace("_", "-").Replace(" ", "-").ToLower()}";
        }

        #endregion
    }
}