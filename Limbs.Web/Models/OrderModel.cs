using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Limbs.Web.Models
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Person who request the order
        /// </summary>
        public UserModel OrderRequestor { get; set; }

        /// <summary>
        /// Person who will use the order
        /// </summary>
        public UserModel OrderUser { get; set; }

        //public virtual ICollection<AccessoryModel> Type { get; set; }

        public ProductModel Product { get; set; }

        public string Comments { get; set; }

        public string Status { get; set; }
    }

    public class ProductModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nombre", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Name { get; set; }

        [Display(Name = "Tipo", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Type { get; set; }

        [Display(Name = "Thumbnail", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Thumbnail { get; set; }

        //public int Version { get; set; }

        //public virtual ICollection<FileModel> Files { get; set; }
    }

    public enum ProductType
    {
        [Description("Derecha")]
        Right,
        [Description("Izquierda")]
        Left,
        [Description("Ambas")]
        Both,
    }

    public static class AttributesHelperExtension
    {
        public static string ToDescription(this Enum value)
        {
            var da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return da.Length > 0 ? da[0].Description : value.ToString();
        }
    }

    /*
    public class FileModel
    {
        [Key]
        public int Id { get; set; }

        public string Url { get; set; }

        public string Descripcion { get; set; }

        public virtual ICollection<FileModel> Products { get; set; }

    }

    public class AccessoryModel
    {
        public AccessoryModel()
        {
            Color = new HashSet<Color>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Price { get; set; }

        public string ImageUrl { get; set; }

        public virtual ICollection<Color> Color { get; set; }
    }

    public class Color
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public virtual AccessoryModel Accessory { get; set; }
    }
    */

    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nombre", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Name { get; set; }

        [Display(Name = "Apellido", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string LastName { get; set; }

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
        public string Gender { get; set; }

        [Display(Name = "País", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Country { get; set; }

        [Display(Name = "Ciudad", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string City { get; set; }

        [Display(Name = "Dirección", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Address { get; set; }

        public double Lat { get; set; }

        public double Long { get; set; }
    }
}