namespace TaxiCameBack.Website.Application.Recaptcha
{
    internal class RecaptchaResponse
    {
        public static readonly RecaptchaResponse Valid = new RecaptchaResponse(true, string.Empty);

        public static readonly RecaptchaResponse CaptchaRequired = new RecaptchaResponse(false, "captcha-required");

        public static readonly RecaptchaResponse InvalidCaptcha = new RecaptchaResponse(false, "incorrect-captcha");

        public static readonly RecaptchaResponse RecaptchaNotReachable = new RecaptchaResponse(false, "recaptcha-not-reachable");

        private bool isValid;

        private string errorCode;

        public bool IsValid
        {
            get
            {
                return this.isValid;
            }
        }

        public string ErrorCode
        {
            get
            {
                return this.errorCode;
            }
        }

        internal RecaptchaResponse(bool isValid, string errorCode)
        {
            this.isValid = isValid;
            this.errorCode = errorCode;
        }

        public override bool Equals(object obj)
        {
            RecaptchaResponse recaptchaResponse = (RecaptchaResponse)obj;
            return recaptchaResponse != null && recaptchaResponse.IsValid == this.IsValid && recaptchaResponse.ErrorCode == this.ErrorCode;
        }

        public override int GetHashCode()
        {
            return this.IsValid.GetHashCode() ^ this.ErrorCode.GetHashCode();
        }
    }
}