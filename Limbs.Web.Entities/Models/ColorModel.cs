﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Limbs.Web.Entities.Models
{
    public class ColorModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int AmputationTypeId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string PrimaryUrlImage { get; set; }
        public string SecondaryUrlImage { get; set; }

        public List<OrderModel> Orders { get; set; }
        public AmputationTypeModel AmputationType { get; set; }
    }
}
