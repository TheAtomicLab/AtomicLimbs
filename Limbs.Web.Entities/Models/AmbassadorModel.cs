using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Web.Mvc;

namespace Limbs.Web.Entities.Models
{

    public class AmbassadorModel
    {
        public AmbassadorModel()
        {
            Gender = Gender.Otro;
        }

        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Email { get; set; }

        [Display(Name = "Nombre", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string AmbassadorName { get; set; }

        [Display(Name = "Apellido", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string AmbassadorLastName { get; set; }

        [DataType("datetime2")]
        [Display(Name = "Fecha de nacimiento", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public DateTime Birth { get; set; }

        [Display(Name = "Sexo", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public Gender Gender { get; set; }

        [Display(Name = "País", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Country { get; set; }

        [Display(Name = "Ciudad", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string City { get; set; }

        [Display(Name = "Dirección", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Address { get; set; }

        [Display(Name = "Teléfono", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Phone { get; set; }

        [Display(Name = "Documento de identidad o pasaporte", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Dni { get; set; }

        public DbGeography Location { get; set; }
        
        public virtual ICollection<OrderModel> OrderModel { get; set; } = new List<OrderModel>();

        public string FullName()
        {
            return $"{AmbassadorName} {AmbassadorLastName}";
        }
    }
}