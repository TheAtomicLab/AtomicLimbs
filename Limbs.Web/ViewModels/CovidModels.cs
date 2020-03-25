using Limbs.Web.Entities.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Globalization;

namespace Limbs.Web.ViewModels
{
    public class CreateCovidOrganizationViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Seleccione una representación", Description = "")]
        [Required(ErrorMessage = " ")]
        public CovidOrganizationEnum CovidOrganizationEnum { get; set; }

        [Display(Name = "Nombre de la organización", Description = "")]
        public string CovidOrganizationName { get; set; }

        [Required(ErrorMessage = " ")]
        public string Email { get; set; }

        [Display(Name = "Nombre", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Name { get; set; }

        [Display(Name = "Apellido", Description = "")]
        [Required(ErrorMessage = " ")]
        public string Lastname { get; set; }

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

        [Range(1, int.MaxValue, ErrorMessage = " ")]
        public int Quantity { get; set; }

        [Display(Name = "Fecha de entrega", Description = " ")]
        [Required(ErrorMessage = " ")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }

        public string Token { get; set; }

        public DbGeography Location { get; set; }

        public string LatLng
        {
            get => $"{Location?.Latitude},{Location?.Longitude}";
            set => Location = GeneratePoint(value?.Split(','));
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