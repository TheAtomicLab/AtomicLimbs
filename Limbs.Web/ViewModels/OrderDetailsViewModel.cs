using Limbs.Web.Entities.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.ViewModels
{
    public class OrderDetailsViewModel
    {
        [Display(Name = "Pedido #", Description = "")]
        public int Id { get; set; }

        public AmputationTypeDetailsViewModel AmputationType { get; set; }

        public ColorDetailsViewModel Color { get; set; }

        [Display(Name = "¿Cuál?", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public ProductType ProductType { get; set; }

        [Display(Name = "Tamaños", Description = "")]
        public OrderSizesModel Sizes { get; set; }

        [Display(Name = "Estado", Description = "")]
        public OrderStatus Status { get; set; }
        public int PercentagePrinted { get; set; }

        [Display(Name = "Comentarios", Description = "")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public string[] Images { get; set; }

        public string FileUrl { get; set; }
        public OrderRequesterDetailsViewModel OrderRequester { get; set; }

        [Display(Name = "Courier", Description = "")]
        public Courier DeliveryCourier { get; set; }

        [Display(Name = "Prueba de entrega", Description = "")]
        public string ProofOfDelivery { get; set; }

        [Display(Name = "Tracking", Description = "")]
        public string DeliveryTrackingCode { get; set; }

        [Display(Name = "Etiqueta postal", Description = "")]
        public string DeliveryPostalLabel { get; set; }

        [Display(Name = "Historial de cambios", Description = "")]
        public List<OrderLogItem> Log { get; private set; }

        public List<RenderPieceGroupByViewModel> RenderPiecesGroupBy { get; set; }
    }
}