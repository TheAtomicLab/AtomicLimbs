﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Limbs.Web.Entities.WebModels
{
    public class ExternalLoginConfirmationViewModel
    {
        public ExternalLoginConfirmationViewModel()
        {
            EmailConfirmed = false;
        }

        [Required(ErrorMessage = " ")]
        [Display(Name = "Email")]
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
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessage = " ")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = " ")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = " ")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = " ")]
        public string Email { get; set; }

        [Required(ErrorMessage = " ")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
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
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = " ")]
        [StringLength(100, ErrorMessage = "La {0} debe tener por lo menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirma contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña no coincide.")]
        public string ConfirmPassword { get; set; }

        public string RecaptchaPublicKey { get; }
    }

    public class SelectUserOrAmbassador
    {
        [Required]
        [EmailAddress(ErrorMessage = " ")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = " ")]
        [EmailAddress(ErrorMessage = " ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = " ")]
        [StringLength(100, ErrorMessage = "La {0} debe tener por lo menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmá contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña no coincide.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = " ")]
        [EmailAddress(ErrorMessage = " ")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
