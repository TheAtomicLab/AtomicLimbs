using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Limbs.Web.Entities.Models
{
    [Table("EventOrderModels")]
    public class EventOrderModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int EventId { get; set; }

        public bool Participated { get; set; }

        public OrderModel Order { get; set; }
        public EventModel Event { get; set; }
    }
}
