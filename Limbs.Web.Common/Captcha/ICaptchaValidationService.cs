namespace Limbs.Web.Common.Captcha
{
    public interface ICaptchaValidationService
    {
        bool Validate(string response);
    }
}
