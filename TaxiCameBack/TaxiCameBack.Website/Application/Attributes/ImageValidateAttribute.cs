using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TaxiCameBack.Website.Application.Extension;

namespace TaxiCameBack.Website.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ImageValidateAttribute : ValidationAttribute, IClientValidatable
    {
        public List<string> ValidExtensions { get; set; }
        public int FileSize { get; set; }

        private const string AllowExtensionErrorMessage = "Allow only image files.";
        private const string AllowFileSizeErrorMessage = "Please select a image file smallar than 1M.";

        public ImageValidateAttribute(string fileExtension, int filesize)
        {
            ValidExtensions = fileExtension.Split('|').ToList();
            FileSize = filesize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var postedFile = value as HttpPostedFileBase;
            if (postedFile != null)
            {
                //-------------------------------------------
                //  Check the image mime types
                //-------------------------------------------
                if (postedFile.ContentType.ToLower() != "image/jpg" &&
                            postedFile.ContentType.ToLower() != "image/jpeg" &&
                            postedFile.ContentType.ToLower() != "image/pjpeg" &&
                            postedFile.ContentType.ToLower() != "image/gif" &&
                            postedFile.ContentType.ToLower() != "image/x-png" &&
                            postedFile.ContentType.ToLower() != "image/png")
                {
                    return new ValidationResult("Allow only image files.");
                }

                //-------------------------------------------
                //  Check the image extension
                //-------------------------------------------
                if (!ValidExtensions.Contains(Path.GetExtension(postedFile.FileName)))
                {
                    return new ValidationResult("Allow only " + string.Join(",", ValidExtensions.ToArray()) + " image files.");
                }

                //-------------------------------------------
                //  Attempt to read the file and check the first bytes
                //-------------------------------------------
                try
                {
                    if (!postedFile.InputStream.CanRead)
                    {
                        return new ValidationResult("An image file can not be read.");
                    }

                    if (postedFile.ContentLength > FileSize)
                    {
                        return new ValidationResult("Please select a image file smallar than 1M.");
                    }

                    byte[] buffer = new byte[512];
                    postedFile.InputStream.Read(buffer, 0, 512);
                    string content = System.Text.Encoding.UTF8.GetString(buffer);
                    if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                    {
                        return new ValidationResult("Invalid image. Please select again.");
                    }
                }
                catch (Exception)
                {
                    return new ValidationResult("Invalid image. Please select again.");
                }

                //-------------------------------------------
                //  Try to instantiate new Bitmap, if .NET will throw exception
                //  we can assume that it's not a valid image
                //-------------------------------------------

                try
                {
                    using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                    {
                    }
                }
                catch (Exception)
                {
                    return new ValidationResult("Invalid image. Please select again.");
                }

                return ValidationResult.Success;

            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule= new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "validimage"
            };

            var errorMessages = new List<string>
                                    {
                                        AllowExtensionErrorMessage,
                                        AllowFileSizeErrorMessage
                                    };

            rule.ValidationParameters.Add("fileextensions", string.Join(",", ValidExtensions));
            rule.ValidationParameters.Add("filesize", FileSize);
            rule.ValidationParameters.Add("errormessages", errorMessages.ToConcatenatedString());

            yield return rule;
        }
    }
}