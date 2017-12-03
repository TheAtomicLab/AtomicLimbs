using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Security.Principal;
using Microsoft.AspNet.Identity;

namespace Limbs.Web.Entities.Models
{

    public class AmbassadorModel
    {
        public static int MinYear = 18;

        public AmbassadorModel()
        {
            Gender = Gender.NoDeclara;
        }

        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Email { get; set; }

        [Display(Name = "Nombre", Description = "")]
        [Required(ErrorMessage = " ")]
        public string AmbassadorName { get; set; }

        [Display(Name = "Apellido", Description = "")]
        [Required(ErrorMessage = " ")]
        public string AmbassadorLastName { get; set; }

        [DataType("datetime2", ErrorMessage = "Fecha inválida")]
        [Display(Name = "Fecha de nacimiento", Description = "")]
        [Required(ErrorMessage = " ")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birth { get; set; }

        [Display(Name = "Género", Description = "")]
        [Required(ErrorMessage = " ")]
        public Gender Gender { get; set; }

        [Display(Name = "País", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Country { get; set; }

        [Display(Name = "Ciudad", Description = "")]
        [Required(ErrorMessage = " ")]
        public string City { get; set; }

        [Display(Name = "Dirección", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Address { get; set; }

        [Display(Name = "Teléfono", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Phone { get; set; }

        [Display(Name = "Documento de identidad o pasaporte", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Dni { get; set; }

        public DbGeography Location { get; set; }
        
        public virtual ICollection<OrderModel> OrderModel { get; set; } = new List<OrderModel>();

        public string FullName()
        {
            return $"{AmbassadorName} {AmbassadorLastName}";
        }

        public bool CanViewOrEdit(IPrincipal user)
        {
            if (user.IsInRole(AppRoles.Administrator))
            {
                return true;
            }

            return UserId == user.Identity.GetUserId();
        }
    }
}