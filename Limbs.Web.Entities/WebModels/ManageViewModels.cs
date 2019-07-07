using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Limbs.Web.Entities.Resources;

namespace Limbs.Web.Entities.WebModels
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        //public IList<UserLoginInfo> Logins { get; set; }
        //public string PhoneNumber { get; set; }
        //public bool TwoFactor { get; set; }
        //public bool BrowserRemembered { get; set; }
        public UserViewModel User { get; set; }
    }

    public class UserViewModel
    {
        public string UserName { get; set; }
        public string UserLastName { get; set; }
        public string Email { get; set; }
        public DateTime Birth { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Manage_NewPassword_ErrorMessage", MinimumLength = 6, ErrorMessageResourceType = typeof(ModelTexts))]
        [DataType(DataType.Password)]
        [Display(Name = "Manage_NewPassword", ResourceType = typeof(ModelTexts))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Manage_ConfirmPassword", ResourceType = typeof(ModelTexts))]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Manage_ConfirmPassword_ErrorMessage", ErrorMessageResourceType = typeof(ModelTexts))]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Manage_OldPassword", ResourceType = typeof(ModelTexts))]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Manage_NewPassword_ErrorMessage", MinimumLength = 6, ErrorMessageResourceType = typeof(ModelTexts))]
        [DataType(DataType.Password)]
        [Display(Name = "Manage_NewPassword", ResourceType = typeof(ModelTexts))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Manage_ConfirmPassword", ResourceType = typeof(ModelTexts))]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Manage_ConfirmPassword_ErrorMessage", ErrorMessageResourceType = typeof(ModelTexts))]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Manage_Number", ResourceType = typeof(ModelTexts))]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Manage_Code", ResourceType = typeof(ModelTexts))]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Manage_Number", ResourceType = typeof(ModelTexts))]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}