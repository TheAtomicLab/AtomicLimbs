using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Limbs.Web.Entities.Models
{
    [Table("CovidOrgAmbassadorModels")]
    public class CovidOrgAmbassador
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CovidAmbassadorId { get; set; }
        public int CovidOrgId { get; set; }
        public int Quantity { get; set; }

        public COVIDEmbajadorEntregable CovidAmbassador { get; set; }
        public CovidOrganizationModel CovidOrg { get; set; }
    }
}
