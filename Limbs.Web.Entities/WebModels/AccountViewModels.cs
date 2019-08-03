using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Limbs.Web.Entities.Resources;

namespace Limbs.Web.Entities.WebModels
{
    public class ExternalLoginConfirmationViewModel
    {
        public ExternalLoginConfirmationViewModel()
        {
            EmailConfirmed = false;
        }

        [Required(ErrorMessage = " ")]
        [Display(Name = "Account_Email", ResourceType = typeof(ModelTexts))]
        [EmailAddress(ErrorMessage = " ")]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Account_Code", ResourceType = typeof(ModelTexts))]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Account_RememberBrowser", ResourceType = typeof(ModelTexts))]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessage = " ")]
        [Display(Name = "Account_Email", ResourceType = typeof(ModelTexts))]
        [EmailAddress(ErrorMessage = " ")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = " ")]
        [Display(Name = "Account_Email", ResourceType = typeof(ModelTexts))]
        [EmailAddress(ErrorMessage = " ")]
        public string Email { get; set; }

        [Required(ErrorMessage = " ")]
        [DataType(DataType.Password)]
        [Display(Name = "Account_Password", ResourceType = typeof(ModelTexts))]
        public string Password { get; set; }

        [Display(Name = "Account_RememberMe", ResourceType = typeof(ModelTexts))]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            RecaptchaPublicKey = ConfigurationManager.AppSettings["Google.Recaptcha.PublicKey"];
        }

        [Required(ErrorMessage = " ")]
        [EmailAddress(ErrorMessage = " ")]
        [Display(Name = "Account_Email", ResourceType = typeof(ModelTexts))]
        public string Email { get; set; }

        [Required(ErrorMessage = " ")]
        [StringLength(100, ErrorMessageResourceName = "Account_Password_ErrorMessage", MinimumLength = 6, ErrorMessageResourceType = typeof(ModelTexts))]
        [DataType(DataType.Password)]
        [Display(Name = "Account_Password", ResourceType = typeof(ModelTexts))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Account_ConfirmPassword", ResourceType = typeof(ModelTexts))]
        [Compare("Password", ErrorMessageResourceName = "Account_ConfirmPassword_ErrorMessage", ErrorMessageResourceType = typeof(ModelTexts))]
        public string ConfirmPassword { get; set; }

        public string RecaptchaPublicKey { get; }
    }

    public class SelectUserOrAmbassador
    {
        [Required]
        [EmailAddress(ErrorMessage = " ")]
        [Display(Name = "Account_Email", ResourceType = typeof(ModelTexts))]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = " ")]
        [EmailAddress(ErrorMessage = " ")]
        [Display(Name = "Account_Email", ResourceType = typeof(ModelTexts))]
        public string Email { get; set; }

        [Required(ErrorMessage = " ")]
        [StringLength(100, ErrorMessageResourceName = "Account_Password_ErrorMessage", MinimumLength = 6, ErrorMessageResourceType = typeof(ModelTexts))]
        [DataType(DataType.Password)]
        [Display(Name = "Account_Password", ResourceType = typeof(ModelTexts))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Account_ConfirmPassword", ResourceType = typeof(ModelTexts))]
        [Compare("Password", ErrorMessageResourceName = "Account_ConfirmPassword_ErrorMessage", ErrorMessageResourceType = typeof(ModelTexts))]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = " ")]
        [EmailAddress(ErrorMessage = " ")]
        [Display(Name = "Account_Email", ResourceType = typeof(ModelTexts))]
        public string Email { get; set; }
    }
}
