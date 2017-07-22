using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Limbs.Web.Models
{

    public class AmbassadorModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nombre completo", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string AmbassadorName { get; set; }

        [Display(Name = "Correo electrónico", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        [EmailAddress]
        public string Email { get; set; }

        [DataType("datetime2")]
        [Display(Name = "Fecha de nacimiento", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birth { get; set; }

        [Display(Name = "Sexo", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public Gender Gender { get; set; }

        [Display(Name = "Dirección", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Address { get; set; }

        [Display(Name = "Documento de identidad o pasaporte", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Dni { get; set; }

        public double Lat { get; set; }

        public double Long { get; set; }

        /*
                --Leave comments for possible evolution-#idEvolution = 1#--lucaslopezf--##

        [Display(Name = "AtributoEmbajador1", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string AtributoEmbajador1 { get; set; }

        [Display(Name = "AtributoEmbajador2", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string AtributoEmbajador2 { get; set; }

        [Display(Name = "AtributoEmbajador3", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string AtributoEmbajador3 { get; set; }

             
    */

        /*
        public double Lat { get; set; }

        public double Long { get; set; }

        [Display(Name = "Teléfono", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Phone { get; set; }

        [Display(Name = "País", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Country { get; set; }

        [Display(Name = "Ciudad", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string City { get; set; }
        */
    }
}