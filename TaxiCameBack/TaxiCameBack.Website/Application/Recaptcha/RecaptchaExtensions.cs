using System.Web;
using System.Web.Mvc;

namespace TaxiCameBack.Website.Application.Recaptcha
{
    public static class RecaptchaExtensions
    {
        public static IHtmlString Recaptcha(this HtmlHelper htmlHelper, string publicKey = "[reCaptchaPublicKey]", CaptchaTheme theme = CaptchaTheme.Light, CaptchaType type = CaptchaType.Image, string callback = "", string expiredCallback = "")
        {
            RecaptchaHtmlHelper recaptchaHtmlHelper = new RecaptchaHtmlHelper(publicKey, theme, type, callback, expiredCallback);
            return htmlHelper.Raw(recaptchaHtmlHelper.ToString());
        }
    }
}