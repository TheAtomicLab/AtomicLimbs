using System.ComponentModel.DataAnnotations;
using Limbs.Web.Resources;

namespace Limbs.Web.ViewModels
{
    public class OrderRequesterDetailsViewModel
    {
        [Display(Name = "OrderRequesterDetails_UserName", Description = "", ResourceType = typeof(ModelsTexts))]
        [Required(ErrorMessage = " ")]
        public string UserName { get; set; }

        [Display(Name = "OrderRequesterDetails_UserLastName", Description = "", ResourceType = typeof(ModelsTexts))]
        [Required(ErrorMessage = " ")]
        public string UserLastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Display(Name = "OrderRequesterDetails_Country", Description = "", ResourceType = typeof(ModelsTexts))]
        [Required(ErrorMessage = " ")]
        public string Country { get; set; }

        [Display(Name = "OrderRequesterDetails_City", Description = "", ResourceType = typeof(ModelsTexts))]
        [Required(ErrorMessage = " ")]
        public string City { get; set; }

        public string FullName { get; set; }
        public string FullDni { get; set; }
        public string FullAddress { get; set; }
        public string Birth { get; set; }

        [Display(Name = "OrderRequesterDetails_Phone", Description = "", ResourceType = typeof(ModelsTexts))]
        [Required(ErrorMessage = " ")]
        public string Phone { get; set; }
    }
}