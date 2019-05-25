using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.Entities.Models
{
    public class AmputationTypeModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PrimaryUrlImage { get; set; }
        public string SecondaryUrlImage { get; set; }

        public List<AmputationTypeColorModel> AmputationTypeColors { get; set; }

        public List<RenderModel> Renders { get; set; }
        public List<ColorModel> Colors { get; set; }
    }
}
