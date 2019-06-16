using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Limbs.Web.Entities.Models
{
    public class RenderModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int AmputationTypeId { get; set; }
        public string Name { get; set; }
        public string PrimaryUrlImage { get; set; }
        public string SecondaryUrlImage { get; set; }

        public AmputationTypeModel AmputationType { get; set; }
        public List<RenderPieceModel> Pieces { get; set; }
    }
}
