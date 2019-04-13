using Limbs.Web.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Limbs.Web.ViewModels
{
    public class OrderUpdateModel
    {
        public int Id { get; set; }

        [Display(Name = "¿Cuál?", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public ProductType ProductType { get; set; }

        [Display(Name = "Amputación", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public AmputationType AmputationType { get; set; }

        [Display(Name = "Comentarios", Description = "")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [Display(Name = "Color", Description = "(si es posible)")]
        public OrderColor Color { get; set; }
    }
}