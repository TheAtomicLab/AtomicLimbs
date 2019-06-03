using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Limbs.Web.ViewModels
{
    public class RenderViewModel
    {
        public int Id { get; set; }
        public int AmputationTypeId { get; set; }
        public string Name { get; set; }
        public string PrimaryUrlImage { get; set; }
        public string SecondaryUrlImage { get; set; }
    }
}