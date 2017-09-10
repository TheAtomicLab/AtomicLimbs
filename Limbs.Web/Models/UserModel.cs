using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Limbs.Web.Models
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

        /*
        [Display(Name = "Correo electrónico", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        [EmailAddress]
        public string Email { get; set; }
        */

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

        [Display(Name = "Tipo de prótesis", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public ProthesisType ProthesisType { get; set; }

        [Display(Name = "Cuál", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public ProductType ProductType { get; set; }

        
        [Display(Name = "Amputación", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public AmputationType AmputationType { get; set; }
        

        public double Lat { get; set; }

        public double Long { get; set; }

        public virtual ICollection<OrderModel> OrderModel { get; set; }

        public static IEnumerable<SelectListItem> GetGenderSelect()
        {
            yield return new SelectListItem { Text = "Masculino", Value = "Hombre" };
            yield return new SelectListItem { Text = "Femenino", Value = "Mujer" };
            yield return new SelectListItem { Text = "No Declara", Value = "Otro" };
        }

        //   public virtual ICollection<int> OrderModelId { get; set; }

        //public int OrderModelId { get; set; }
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

    public enum AmputationType
    {
        [Description("Perdí una falange de cualquier dedo")]
        A,
        [Description("Perdí dos falanges de cualquier dedo")]
        B,
        [Description("Perdí mis cuatro dedos y tengo un pulgar")]
        C,
        [Description("Perdí el pulgar y no tengo los dedos (Poseo hueso capital)")]
        D,
        [Description("Perdí dos falanges de cualquier dedo")]
        E,
        [Description("Perdí dos falanges de cualquier dedo")]
        F,
        [Description("Perdí dos falanges de cualquier dedo")]
        G,
        [Description("Perdí dos falanges de cualquier dedo")]
        H
    }
}