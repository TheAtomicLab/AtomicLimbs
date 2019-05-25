using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.Entities.Models
{
    public class ColorModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public List<AmputationTypeColorModel> AmputationTypeColors { get; set; }
    }
}
