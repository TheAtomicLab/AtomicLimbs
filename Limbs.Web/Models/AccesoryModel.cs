using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Limbs.Web.Models
{
    
    public class AccessoryModel
    {
        public AccessoryModel()
        {
            Color = new HashSet<Color>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Price { get; set; }

        public string ImageUrl { get; set; }

        public virtual ICollection<Color> Color { get; set; }
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