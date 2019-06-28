using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Limbs.Web.Entities.Models
{
    public class AmputationTypeModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Short_Description { get; set; }
        public string PrimaryUrlImage { get; set; }
        public string SecondaryUrlImage { get; set; }

        public List<OrderModel> Orders { get; set; }
        public List<RenderModel> Renders { get; set; }
        public List<ColorModel> Colors { get; set; }
    }
}
