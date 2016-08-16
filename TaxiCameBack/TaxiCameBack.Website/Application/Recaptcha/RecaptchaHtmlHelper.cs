using System;
using System.IO;
using System.Web.UI;

namespace TaxiCameBack.Website.Application.Recaptcha
{
    internal class RecaptchaHtmlHelper
    {
        public string PublicKey
        {
            get;
            private set;
        }

        public string PrivateKey
        {
            get;
            private set;
        }

        public CaptchaTheme Theme
        {
            get;
            private set;
        }

        public CaptchaType Type
        {
            get;
            private set;
        }

        public string Callback
        {
            get;
            private set;
        }

        public string ExpiredCallback
        {
            get;
            private set;
        }

        public RecaptchaHtmlHelper(string publicKey, CaptchaTheme theme, CaptchaType type, string callback, string expiredCallback)
        {
            this.PublicKey = RecaptchaKeyHelper.ParseKey(publicKey);
            this.Theme = theme;
            this.Type = type;
            this.Callback = callback;
            this.ExpiredCallback = expiredCallback;
            if (string.IsNullOrEmpty(publicKey))
            {
                throw new InvalidOperationException("Public key cannot be null or empty.");
            }
        }

        public override string ToString()
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter htmlTextWriter = new HtmlTextWriter(stringWriter))
            {
                htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
                htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Src, "//www.google.com/recaptcha/api.js");
                htmlTextWriter.AddAttribute("async", null);
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Script);
                htmlTextWriter.RenderEndTag();
                htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Class, "g-recaptcha");
                htmlTextWriter.AddAttribute("data-sitekey", this.PublicKey);
                htmlTextWriter.AddAttribute("data-theme", this.Theme.ToString().ToLower());
                htmlTextWriter.AddAttribute("data-type", this.Type.ToString().ToLower());
                if (!string.IsNullOrWhiteSpace(this.Callback))
                {
                    htmlTextWriter.AddAttribute("data-callback", this.Callback);
                }
                if (!string.IsNullOrWhiteSpace(this.ExpiredCallback))
                {
                    htmlTextWriter.AddAttribute("data-expired-callback", this.ExpiredCallback);
                }
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Div);
                htmlTextWriter.RenderEndTag();
            }
            return stringWriter.ToString();
        }
    }
}