using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Globalization;
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
            RegisteredAt = DateTime.UtcNow;
        }

        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Email { get; set; }

        [Display(Name = "Email Alternativo", Description = "")]
        [EmailAddress(ErrorMessage = " ")]
        public string AlternativeEmail { get; set; }

        public DateTime? RegisteredAt { get; set; }

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

        [Display(Name = "Teléfono", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Phone { get; set; }

        [Display(Name = "Documento de identidad o pasaporte", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Dni { get; set; }

        public override string ToString()
        {
            var separator = ",";

            List<String> listAmbassador = new List<String>
            {
                this.Id.ToString(),
                this.UserId,
                this.Dni,
                this.Email,
                this.AlternativeEmail,
                String.Concat("\"",this.AmbassadorName,"\""),
                this.AmbassadorLastName,
                this.Phone,
                this.Birth.ToString(),
                this.Gender.ToString(),
                this.Country,
                this.State,
                this.City,
                String.Concat("\"",this.Address,"\""),
                String.Concat("\"",this.Address2,"\""),
                this.RegisteredAt.ToString(),
            };

            return String.Join(separator, listAmbassador);
        }

        public DbGeography Location { get; set; }
        
        [NotMapped]
        public string LatLng
        {
            get => $"{Location?.Latitude},{Location?.Longitude}";
            set => Location = GeneratePoint(value?.Split(','));
        }

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

        private DbGeography GeneratePoint(string[] point)
        {
            return (point == null || point.Length != 2) ? null : GeneratePoint(double.Parse(point[0]), double.Parse(point[1]));
        }

        private DbGeography GeneratePoint(double lat, double lng)
        {
            return DbGeography.PointFromText("POINT(" + lng.ToString("G17", CultureInfo.InvariantCulture) + " " + lat.ToString("G17", CultureInfo.InvariantCulture) + ")", 4326);
        }
    }
}