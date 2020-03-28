using Limbs.Web.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.ViewModels
{
    public class CovidUpdateQuantity
    {
        public int CovidOrgAmbassadorId { get; set; }
        public int SavedQuantity { get; set; }
        public int OrgId { get; set; }
        public int CovidAmbassadorId { get; set; }
    }

    public class CovidEmbajadorEntregableViewModel
    {
        public int Id { get; set; }
        public int AmbassadorId { get; set; }
        public int TipoEntregable { get; set; }

        [Required(ErrorMessage = " "), Range(0, int.MaxValue, ErrorMessage = " ")]
        public int CantEntregable { get; set; }

        public List<OrderCovidAmbassadorViewModel> Orders { get; set; }
    }

    public class OrderCovidAmbassadorViewModel
    {
        public int OrgId { get; set; }
        public OrderCovidInfoViewModel OrderInfo { get; set; }
    }

    public class OrderCovidInfoViewModel
    {
        public CovidOrganizationEnum CovidOrganization { get; set; }
        public string CovidOrganizationName { get; set; }
        public int Quantity { get; set; }
        public DateTime DeliveryDate { get; set; }
        public double Distance { get; set; }


        public List<CovidAmbassador> Ambassadors { get; set; }
        public bool AlreadySavedQuantity { get; set; }
        public int? QuantitySaved { get; set; }
    }

    public class CovidAmbassador
    {
        public string Name { get; set; }
        public string Lastname { get; set; }

        public int Quantity { get; set; }
    }
}