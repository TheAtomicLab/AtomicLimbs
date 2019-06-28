using Limbs.Web.Entities.Models;
using Limbs.Web.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.ViewModels
{
    public class OrderDetailsViewModel
    {
        [Display(Name = "OrderDetails_Id", Description = "", ResourceType = typeof(ModelsTexts))]
        public int Id { get; set; }

        public AmputationTypeDetailsViewModel AmputationType { get; set; }

        public ColorDetailsViewModel Color { get; set; }

        [Display(Name = "OrderDetails_ProductType", Description = "", ResourceType = typeof(ModelsTexts))]
        [Required(ErrorMessage = "RequiredErrorMessage", ErrorMessageResourceType = typeof(ModelsTexts))]
        public ProductType ProductType { get; set; }

        [Display(Name = "OrderDetails_Sizes", Description = "", ResourceType = typeof(ModelsTexts))]
        public OrderSizesModel Sizes { get; set; }

        [Display(Name = "OrderDetails_Status", Description = "", ResourceType = typeof(ModelsTexts))]
        public OrderStatus Status { get; set; }
        public int PercentagePrinted { get; set; }

        [Display(Name = "OrderDetails_Comments", Description = "", ResourceType = typeof(ModelsTexts))]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public string[] Images { get; set; }

        public string FileUrl { get; set; }
        public OrderRequesterDetailsViewModel OrderRequester { get; set; }

        [Display(Name = "OrderDetails_DeliveryCourier", Description = "", ResourceType = typeof(ModelsTexts))]
        public Courier DeliveryCourier { get; set; }

        [Display(Name = "OrderDetails_ProofOfDelivery", Description = "", ResourceType = typeof(ModelsTexts))]
        public string ProofOfDelivery { get; set; }

        [Display(Name = "OrderDetails_DeliveryTrackingCode", Description = "", ResourceType = typeof(ModelsTexts))]
        public string DeliveryTrackingCode { get; set; }

        [Display(Name = "OrderDetails_DeliveryPostalLabel", Description = "", ResourceType = typeof(ModelsTexts))]
        public string DeliveryPostalLabel { get; set; }

        public bool HasDesign { get; set; }

        [Display(Name = "OrderDetails_Log", Description = "", ResourceType = typeof(ModelsTexts))]
        public List<OrderLogItem> Log { get; private set; }

        public List<RenderPieceGroupByViewModel> RenderPiecesGroupBy { get; set; }
    }
}