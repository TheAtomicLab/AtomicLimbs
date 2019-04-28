using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Limbs.Web.Entities.Models
{
    public class OrderRefusedModels
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int AmbassadorId { get; set; }
        public DateTime Date { get; set; }

        public OrderModel Order { get; set; }
        public AmbassadorModel Ambassador { get; set; }
    }
}
