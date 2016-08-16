using System.Collections.Generic;
using Newtonsoft.Json;

namespace TaxiCameBack.Website.Application.Recaptcha
{
    internal class CaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success
        {
            get;
            set;
        }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes
        {
            get;
            set;
        }
    }
}