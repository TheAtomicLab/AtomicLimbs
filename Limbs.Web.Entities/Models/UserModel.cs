using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Security.Principal;
using Microsoft.AspNet.Identity;

namespace Limbs.Web.Entities.Models
{

    public class UserModel
    {
        private string _responsableName;
        private string _responsableLastName;

        public UserModel()
        {
            Gender = Gender.NoDeclara;
            RegisteredAt = DateTime.UtcNow;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Email { get; set; }

        public DateTime? RegisteredAt { get; set; }

        /// <summary>
        /// If the responsable = product user
        /// </summary>
        public bool IsProductUser { get; set; }

        [Display(Name = "Nombre del usuario", Description = "")]
        [Required(ErrorMessage = " ")]
        public string UserName { get; set; }

        [Display(Name = "Apellido del usuario", Description = "")]
        [Required(ErrorMessage = " ")]
        public string UserLastName { get; set; }

        [Display(Name = "Nombre del responsable", Description = "")]
        public string ResponsableName
        {
            get => IsProductUser ? null : _responsableName;
            set => _responsableName = IsProductUser ? null : value;
        }

        [Display(Name = "Apellido del responsable", Description = "")]
        public string ResponsableLastName
        {
            get => IsProductUser ? null : _responsableLastName;
            set => _responsableLastName = IsProductUser ? null : value;
        }

        [Display(Name = "Teléfono", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Phone { get; set; }

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

        [Display(Name = "Provincia", Description = "")]
        [Required(ErrorMessage = " ")]
        public string State { get; set; }

        [Display(Name = "Ciudad", Description = "")]
        [Required(ErrorMessage = " ")]
        public string City { get; set; }

        [Display(Name = "Dirección (solo calle y altura)", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Address { get; set; }

        [Display(Name = "Dirección (otros datos)", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Address2 { get; set; }

        [Display(Name = "DNI o Pasaporte del usuario", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Dni { get; set; }
        
        public DbGeography Location { get; set; }

        [NotMapped]
        public string LatLng
        {
            get => $"{Location?.Latitude},{Location?.Longitude}";
            set => GeneratePoint(value.Split(','));
        }

        public virtual ICollection<OrderModel> OrderModel { get; set; }

        public string Name => IsProductUser ? UserName : ResponsableName;

        public string LastName => IsProductUser ? UserLastName : ResponsableLastName;

        public string FullName()
        {
            var result = $"{UserName} {UserLastName}";
            if (!IsProductUser)
                result += $" (Responsable: {ResponsableName} {ResponsableLastName})";
            return result;
        }

        public string FullNameWithoutLastName()
        {
            var result = $"{UserName}";
            if (!IsProductUser)
                result += $" (Responsable: {ResponsableName})";
            return result;
        }

        public string FullAddress()
        {
            return $"{Address} {Address2}, {City} {State}, {Country}";
        }

        public bool CanViewOrEdit(IPrincipal user)
        {
            if (user.IsInRole(AppRoles.Administrator))
            {
                return true;
            }

            return UserId == user.Identity.GetUserId();
        }

        private DbGeography GeneratePoint(string[] point)
        {
            return (point == null || point.Length != 2) ? null : GeneratePoint(double.Parse(point[0]), double.Parse(point[1]));
        }

        private DbGeography GeneratePoint(double lat, double lng)
        {
            return DbGeography.PointFromText("POINT(" + lng.ToString("G17", CultureInfo.InvariantCulture) + " " + lat.ToString("G17", CultureInfo.InvariantCulture) + ")", 4326);
        }
    }
    
    public enum Gender
    {
        [Description("Femenino")]
        Mujer = 0,
        [Description("Masculino")]
        Hombre = 1,
        [Description("Otro")]
        Otro = 2,
        [Description("No Declara")]
        NoDeclara = 3,
    }
}