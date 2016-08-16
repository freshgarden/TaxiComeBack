using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace TaxiCameBack.Website.Application.Recaptcha
{
    internal class RecaptchaValidator
    {
        private const string VerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

        private string privateKey;

        private string remoteIp;

        private string response;

        public string PrivateKey
        {
            get
            {
                return privateKey;
            }
            set
            {
                privateKey = value;
            }
        }

        public string RemoteIP
        {
            get
            {
                return remoteIp;
            }
            set
            {
                IPAddress iPAddress = IPAddress.Parse(value);
                if (iPAddress == null || (iPAddress.AddressFamily != AddressFamily.InterNetwork && iPAddress.AddressFamily != AddressFamily.InterNetworkV6))
                {
                    throw new ArgumentException("Expecting an IP address, got " + iPAddress);
                }
                remoteIp = iPAddress.ToString();
            }
        }

        public string Response
        {
            get
            {
                return response;
            }
            set
            {
                response = value;
            }
        }

        private void CheckNotNull(object obj, string name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public RecaptchaResponse Validate()
        {
            CheckNotNull(PrivateKey, "PrivateKey");
            CheckNotNull(RemoteIP, "RemoteIp");
            if (string.IsNullOrWhiteSpace(response))
            {
                return RecaptchaResponse.CaptchaRequired;
            }
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(VerifyUrl);
            httpWebRequest.ProtocolVersion = HttpVersion.Version11;
            httpWebRequest.Timeout = 30000;
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = "reCAPTCHA/ASP.NET";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            string s = string.Format("secret={0}&response={1}&remoteip={2}", HttpUtility.UrlEncode(PrivateKey), HttpUtility.UrlEncode(Response), HttpUtility.UrlEncode(RemoteIP));
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
            CaptchaResponse captchaResponse = new CaptchaResponse();
            try
            {
                using (WebResponse webResponse = httpWebRequest.GetResponse())
                {
                    using (TextReader textReader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                    {
                        captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(textReader.ReadToEnd());
                    }
                }
            }
            catch
            {
                RecaptchaResponse recaptchaNotReachable = RecaptchaResponse.RecaptchaNotReachable;
                return recaptchaNotReachable;
            }
            if (!captchaResponse.Success)
            {
                string errorCode = string.Empty;
                if (captchaResponse.ErrorCodes != null)
                {
                    errorCode = string.Join(",", captchaResponse.ErrorCodes.ToArray());
                }
                return new RecaptchaResponse(false, errorCode);
            }
            return RecaptchaResponse.Valid;
        }
    }
}