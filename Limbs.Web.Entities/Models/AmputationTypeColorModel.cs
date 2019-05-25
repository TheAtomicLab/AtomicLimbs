using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limbs.Web.Entities.Models
{
    public class AmputationTypeColorModel
    {
        [Key]
        public int Id { get; set; }
        public int AmputationTypeId { get; set; }
        public int ColorId { get; set; }

        public string PrimaryUrlImage { get; set; }
        public string SecondaryUrlImage { get; set; }

        public AmputationTypeModel AmputationType { get; set; }
        public ColorModel Color { get; set; }
    }
}
