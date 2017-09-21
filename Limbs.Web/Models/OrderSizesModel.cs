using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Limbs.Web.Models
{
    public class OrderSizesModel
    {
        [Key]
        public int Id { get; set; }

        public float A { get; set; }

        public float B { get; set; }

        public float C { get; set; }

        public int D { get; set; }
    }

}