using System.Web.Mvc;

namespace TaxiCameBack.Website.Application.Recaptcha
{
    public class CaptchaValidatorAttribute : ActionFilterAttribute
    {
        public string ErrorMessage
        {
            get;
            set;
        }

        public string RequiredMessage
        {
            get;
            set;
        }

        public string PrivateKey
        {
            get;
            set;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RecaptchaValidator recaptchaValidator = new RecaptchaValidator
            {
                PrivateKey = (string.IsNullOrWhiteSpace(PrivateKey) ? RecaptchaKeyHelper.ParseKey("[reCaptchaPrivateKey]") : this.PrivateKey),
                RemoteIP = filterContext.HttpContext.Request.UserHostAddress,
                Response = filterContext.HttpContext.Request.Form["g-recaptcha-response"]
            };
            RecaptchaResponse recaptchaResponse = recaptchaValidator.Validate();
            if (!recaptchaResponse.IsValid)
            {
                ((Controller)filterContext.Controller).ModelState.AddModelError("ReCaptcha", this.GetErrorMessage(recaptchaResponse.ErrorCode));
            }
            filterContext.ActionParameters["captchaValid"] = recaptchaResponse.IsValid;
            base.OnActionExecuting(filterContext);
        }

        private string GetErrorMessage(string errorCode)
        {
            string result;
            if (errorCode != null)
            {
                if (errorCode == "captcha-required")
                {
                    result = (string.IsNullOrWhiteSpace(this.RequiredMessage) ? "Captcha field is required." : this.RequiredMessage);
                    return result;
                }
                if (errorCode == "missing-input-secret")
                {
                    result = "The secret parameter is missing.";
                    return result;
                }
                if (errorCode == "invalid-input-secret")
                {
                    result = "The secret parameter is invalid or malformed.";
                    return result;
                }
                if (errorCode == "missing-input-response")
                {
                    result = "The response parameter is missing.";
                    return result;
                }
                if (errorCode == "invalid-input-response")
                {
                    result = (string.IsNullOrWhiteSpace(this.ErrorMessage) ? "Incorrect Captcha" : this.ErrorMessage);
                    return result;
                }
            }
            result = (string.IsNullOrWhiteSpace(this.ErrorMessage) ? "Incorrect Captcha" : this.ErrorMessage);
            return result;
        }
    }
}