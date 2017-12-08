using System.Configuration;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Limbs.Web.Common.Captcha
{
    public class InvisibleRecaptchaValidationService : ICaptchaValidationService
    {
        private const string ApiUrl = "https://www.google.com/recaptcha/api/siteverify";
        private readonly string _secretKey;

        public InvisibleRecaptchaValidationService()
        {
            _secretKey = ConfigurationManager.AppSettings["Google.Recaptcha.SecretKey"];
        }

        public bool Validate(string response)
        {
            if (!string.IsNullOrWhiteSpace(response))
            {
                using (var client = new WebClient())
                {
                    var result = client.DownloadString($"{ApiUrl}?secret={_secretKey}&response={response}");
                    return ParseValidationResult(result);
                }
            }

            return false;
        }

        private bool ParseValidationResult(string validationResult) => (bool)JObject.Parse(validationResult).SelectToken("success");
    }
}
