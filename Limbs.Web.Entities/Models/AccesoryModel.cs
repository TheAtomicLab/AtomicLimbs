using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.Entities.Models
{
    
    public class AccessoryModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Price { get; set; }

        public string ImageUrl { get; set; }

        public virtual ICollection<Color> Color { get; set; } = new HashSet<Color>();
    }

    public class Color
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public virtual AccessoryModel Accessory { get; set; }
    }

}