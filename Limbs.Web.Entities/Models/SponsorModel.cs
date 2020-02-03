using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Limbs.Web.Entities.Models
{
    public class SponsorModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public EventModel Event { get; set; }
        public string WebImage { get; set; }
        public string MobileImage { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
