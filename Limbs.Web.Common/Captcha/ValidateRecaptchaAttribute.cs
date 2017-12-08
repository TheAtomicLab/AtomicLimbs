using System.Web.Mvc;

namespace Limbs.Web.Common.Captcha
{
    public class ValidateRecaptchaAttribute : ActionFilterAttribute
    {
        private const string RecaptchaResponseKey = "g-recaptcha-response";

        public ValidateRecaptchaAttribute()
        {
            CaptchaService = new InvisibleRecaptchaValidationService();
        }

        public ICaptchaValidationService CaptchaService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isValidate = CaptchaService.Validate(filterContext.HttpContext.Request[RecaptchaResponseKey]);
            if (!isValidate)
                filterContext.Controller.ViewData.ModelState.AddModelError("Recaptcha", @"Verifique que no es una máquina.");
        }
    }

}
