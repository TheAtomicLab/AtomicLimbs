using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Limbs.Web.Models
{
   
    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Nombre completo del Responsable", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string ResponsableName { get; set; }
        
        [Display(Name = "Email", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        [EmailAddress]
        public string Email { get; set; }

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

        /*
        [Display(Name = "País", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Country { get; set; }

        [Display(Name = "Ciudad", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string City { get; set; }
        */
        [Display(Name = "Dirección", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Address { get; set; }

        [Display(Name = "DNI o Pasaporte del usuario", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Dni { get; set; }

        [Display(Name = "Nombre completo del usuario", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string UserName { get; set; }

        [Display(Name = "Tipo de prótesis", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public ProthesisType ProthesisType { get; set; }

        [Display(Name = "Cuál", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public ProductType ProductType { get; set; }

        [Display(Name = "Amputación", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string AmputationType { get; set; }
        
        public double Lat { get; set; }

        public double Long { get; set; }
    }

    public enum ProthesisType
    {
        [Description("Mano")]
        Hand,
        [Description("Brazo")]
        Arm
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