using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.ViewModels
{
    public class OrderRequesterDetailsViewModel
    {
        [Display(Name = "Nombre del usuario", Description = "")]
        [Required(ErrorMessage = " ")]
        public string UserName { get; set; }

        [Display(Name = "Apellido del usuario", Description = "")]
        [Required(ErrorMessage = " ")]
        public string UserLastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Display(Name = "País", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Country { get; set; }

        [Display(Name = "Ciudad", Description = "")]
        [Required(ErrorMessage = " ")]
        public string City { get; set; }

        public string FullName { get; set; }
        public string FullDni { get; set; }
        public string FullAddress { get; set; }
        public string Birth { get; set; }

        [Display(Name = "Teléfono", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Phone { get; set; }
    }
}