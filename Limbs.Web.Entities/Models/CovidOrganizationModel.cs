using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Limbs.Web.Entities.Models
{
    [Table("CovidOrganizationModels")]
    public class CovidOrganizationModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public CovidOrganizationEnum CovidOrganization { get; set; }
        public string CovidOrganizationName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Dni { get; set; }
        public string PersonalPhone { get; set; }

        public string OrganizationPhone { get; set; }
        public string OrganizationPhoneIntern { get; set; }

        public string Email { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public string Address2 { get; set; }

        public int Quantity { get; set; }

        public string Token { get; set; }

        public bool Featured { get; set; }

        public DbGeography Location { get; set; }

        public List<CovidOrgAmbassador> CovidOrgAmbassadors { get; set; }
    }

    public enum CovidOrganizationEnum
    {
        [Description("Hospital")] Hospital = 1,
        [Description("Policía")] Policia = 2,
        [Description("Bomberos")] Bomberos = 3,

        [Description("Ente público/gubernamental")]
        EntePublicoGubernamental = 4,
        [Description("Repartidor")] Repartidor = 5,
        [Description("Particular")] Particular = 6,
        [Description("Otro (aclarar)")] Otro = 7,
    }
}