using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Limbs.Web.Entities.Models
{
    [Table("EventUserModels")]
    public class EventUserModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }

        public bool Participated { get; set; }

        public UserModel User { get; set; }
        public EventModel Event { get; set; }
    }
}
