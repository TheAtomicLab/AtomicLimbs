using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using Limbs.Web.Entities.Resources;

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

        [Display(Name = "Email Alternativo", Description = "", ResourceType = typeof(ModelTexts))]
        [EmailAddress(ErrorMessage = " ")]
        public string AlternativeEmail { get; set; }

        public DateTime? RegisteredAt { get; set; }

        [Display(Name = "Nombre", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessage = " ")]
        public string AmbassadorName { get; set; }

        [Display(Name = "Apellido", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessage = " ")]
        public string AmbassadorLastName { get; set; }

        [DataType("datetime2", ErrorMessage = "Fecha inválida", ErrorMessageResourceType = typeof(ModelTexts))]
        [Display(Name = "Fecha de nacimiento", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessage = " ")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birth { get; set; }

        [Display(Name = "Género", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessage = " ")]
        public Gender Gender { get; set; }

        [Display(Name = "Organización", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessage = " ")]
        public Organization Organization { get; set; }

        [Display(Name = "Nombre de la organización", Description = "", ResourceType = typeof(ModelTexts))]
        public string OrganizationName { get; set; }

        [Display(Name = "Rol en la organización", Description = "", ResourceType = typeof(ModelTexts))]
        public string RoleInOrganization { get; set; }

        [Display(Name = "País", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessage = " ")]
        public string Country { get; set; }

        [Display(Name = "Provincia", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessage = " ")]
        public string State { get; set; }

        [Display(Name = "Ciudad", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessage = " ")]
        public string City { get; set; }

        [Display(Name = "Dirección (solo calle y altura)", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessage = " ")]
        public string Address { get; set; }

        [Display(Name = "Dirección (otros datos)", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessage = " ")]
        public string Address2 { get; set; }

        [Display(Name = "Teléfono", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessage = " ")]
        public string Phone { get; set; }

        [Display(Name = "Documento de identidad o pasaporte", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessage = " ")]
        public string Dni { get; set; }

        public bool SendFollowUp { get; set; }

        public ICollection<OrderRefusedModels> RefusedOrders { get; set; }
        public virtual ApplicationUser User { get; set; }

        private string StringToCSVCell(string str)
        {
            if (str == null)
            {
                str = "";
            }

            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                    {
                        sb.Append("\"");
                    }
                }
                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }

        public override string ToString()
        {
            var separator = ",";

            List<String> listAmbassador = new List<String>
            {
                this.Id.ToString(),
                this.UserId,
                StringToCSVCell(this.Dni),
                this.Email,
                this.AlternativeEmail,
                StringToCSVCell(this.AmbassadorName),
                StringToCSVCell(this.AmbassadorLastName),
                StringToCSVCell(this.Phone),
                this.Birth.ToString(),
                this.Gender.ToString(),
                this.Organization.ToString(),
                StringToCSVCell(this.OrganizationName),
                StringToCSVCell(this.RoleInOrganization),
                StringToCSVCell(this.Country),
                StringToCSVCell(this.State),
                StringToCSVCell(this.City),
                StringToCSVCell(this.Address),
                StringToCSVCell(this.Address2),
                this.RegisteredAt.ToString(),
            };

            return String.Join(separator, listAmbassador);
        }

        //TODO: change this
        public List<String> GetTitles()
        {
            List<String> titles = new List<string>
            {
                "AmbassadorId",
                "AmbassadorIdTable",
                "AmbassadorDni",
                "AmbassadorEmail",
                "AmbassadorEmailAlternative",
                "AmbassadorName",
                "AmbassadorLastName",
                "AmbassadorPhone",
                "AmbassadorDate",
                "AmbassadorGender",
                "AmbassadorOrganization",
                "AmbassadorOrganizationName",
                "AmbassadorRoleInOrganization",
                "AmbassadorCountry",
                "AmbassadorState",
                "AmbassadorCity",
                "AmbassadorAddress",
                "AmbassadorAddress2",
                "AmbassadorRegisteredAt"
            };

            return titles;
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

        public bool HasAlternativeEmail()
        {
            return !string.IsNullOrWhiteSpace(AlternativeEmail);
        }
        public PrinterModel Printer { get; set; }
    }
    public class PrinterModel
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? Long { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string PrintingArea { get; set; }
        public bool IsHotBed { get; set; }
    }

    public enum Organization
    {
        [Description("No, por cuenta propia")]
        CuentaPropia = 1,
        [Description("Escuela")]
        Escuela = 2,
        [Description("Universidad")]
        Universidad = 3,
        [Description("Organismo Gubernamental")]
        OrganismoGubernamental = 4,
        [Description("Organización SIN fines de lucro")]
        OrganizacionSinFinesDeLucro = 5,
        [Description("Organización CON fines de lucro")]
        OrganizacionConFinesDeLucro = 6
    }
}