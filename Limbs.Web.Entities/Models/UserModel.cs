using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Web.Mvc;

namespace Limbs.Web.Entities.Models
{

    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Email { get; set; }

        [Display(Name = "Nombre del usuario", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string UserName { get; set; }

        [Display(Name = "Apellido del usuario", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string UserLastName { get; set; }

        [Display(Name = "Nombre del Responsable", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string ResponsableName { get; set; }

        [Display(Name = "Apellido del Responsable", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string ResponsableLastName { get; set; }
        
        [Display(Name = "Teléfono", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Phone { get; set; }

        [DataType("datetime2")]
        [Display(Name = "Fecha de nacimiento", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birth { get; set; }

        [Display(Name = "Genero", Description = "")]
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

        [Display(Name = "DNI o Pasaporte del usuario", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Dni { get; set; }
        
        public DbGeography Location { get; set; }

        public virtual ICollection<OrderModel> OrderModel { get; set; }

        //TODO (ale): sacar esto
        public static IEnumerable<SelectListItem> GetGenderSelect()
        {
            yield return new SelectListItem { Text = "Masculino", Value = "Hombre" };
            yield return new SelectListItem { Text = "Femenino", Value = "Mujer" };
            yield return new SelectListItem { Text = "No Declara", Value = "Otro" };
        }

        public string FullName()
        {
            return $"{UserName} {UserLastName}";
        }
    }
    
    public enum Gender
    {
        [Description("Femenino")]
        Mujer,
        [Description("Masculino")]
        Hombre,
        [Description("No Declara")]
        Otro
    }
}