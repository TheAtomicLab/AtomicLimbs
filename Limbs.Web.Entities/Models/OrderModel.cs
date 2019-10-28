using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Limbs.Web.Entities.Resources;

namespace Limbs.Web.Entities.Models
{
    public class OrderModel
    {
        public OrderModel()
        {
            DeliveryCourier = Courier.NoCourier;
            Log = new List<OrderLogItem>();
            Pieces = new Pieces();
        }

        [Key]
        [Display(Name = "Order_Id", Description = "", ResourceType = typeof(ModelTexts))]
        public int Id { get; set; }

        [Display(Name = "Order_Date", Description = "", ResourceType = typeof(ModelTexts))]
        public DateTime Date { get; set; }

        /// <summary>
        ///     Usuario que solicita la orden de protesis
        /// </summary>
        public UserModel OrderRequestor { get; set; }

        /// <summary>
        ///     Embajador que procesara la orden
        /// </summary>
        public virtual AmbassadorModel OrderAmbassador { get; set; }

        [Display(Name = "Order_Color_Name", Description = "Order_Color_Description", ResourceType = typeof(ModelTexts))]
        public OrderColor Color { get; set; }

        [Display(Name = "Order_ProductType", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessageResourceName = "RequiredErrorMessage", ErrorMessageResourceType = typeof(ModelTexts))]
        public ProductType ProductType { get; set; }

        public virtual ICollection<AccessoryModel> Extras { get; set; }

        public int? AmputationTypeFkId { get; set; }

        public AmputationTypeModel AmputationTypeFk { get; set; }

        public int? ColorFkId { get; set; }
        public ColorModel ColorFk { get; set; }

        public List<OrderRenderPieceModel> RenderPieces { get; set; }

        [NotMapped]
        [Display(Name = "Order_OrderSizesModel", Description = "", ResourceType = typeof(ModelTexts))]
        public virtual OrderSizesModel Sizes { get; set; }

        public string SizesData
        {
            get => JsonConvert.SerializeObject(Sizes);
            set => Sizes = value != null
                ? JsonConvert.DeserializeObject<OrderSizesModel>(value)
                : new OrderSizesModel();
        }

        [Display(Name = "Order_AmputationType", Description = "", ResourceType = typeof(ModelTexts))]
        [Required(ErrorMessageResourceName = "RequiredErrorMessage", ErrorMessageResourceType = typeof(ModelTexts))]
        public AmputationType AmputationType { get; set; }

        [Display(Name = "Order_Comments", Description = "", ResourceType = typeof(ModelTexts))]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [Display(Name = "Order_IdImage", Description = "", ResourceType = typeof(ModelTexts))]
        public string IdImage { get; set; }

        [Display(Name = "Order_Status", Description = "", ResourceType = typeof(ModelTexts))]
        public OrderStatus Status { get; set; }

        [Display(Name = "Order_StatusLastUpdated", Description = "", ResourceType = typeof(ModelTexts))]
        public DateTime StatusLastUpdated { get; set; }

        [Display(Name = "Order_ProofOfDelivery", Description = "", ResourceType = typeof(ModelTexts))]
        public string ProofOfDelivery { get; set; }

        [Display(Name = "Order_DeliveryCourier", Description = "", ResourceType = typeof(ModelTexts))]
        public Courier DeliveryCourier { get; set; }

        [Display(Name = "Order_DeliveryTrackingCode", Description = "", ResourceType = typeof(ModelTexts))]
        public string DeliveryTrackingCode { get; set; }

        [Display(Name = "Order_DeliveryPostalLabel", Description = "", ResourceType = typeof(ModelTexts))]
        public string DeliveryPostalLabel { get; set; }

        [NotMapped]
        [Display(Name = "Order_Log", Description = "", ResourceType = typeof(ModelTexts))]
        public List<OrderLogItem> Log { get; private set; }

        public List<EventOrderModel> EventOrders { get; set; }

        public ICollection<OrderRefusedModels> RefusedOrders { get; set; }
        public virtual ICollection<MessageModel> Messages { get; set; }

        public string OrderLog
        {
            get => JsonConvert.SerializeObject(Log);
            set => Log = value != null
                ? JsonConvert.DeserializeObject<List<OrderLogItem>>(value)
                : new List<OrderLogItem>();
        }

        public void LogMessage(IPrincipal user, string message)
        {
            Log.Add(new OrderLogItem
            {
                User = user.Identity.GetUserName(),
                Message = message
            });
        }

        public void LogMessage(IPrincipal user, string message,string orderString)
        {
            Log.Add(new OrderLogItem
            {
                User = user.Identity.GetUserName(),
                Message = message,
                OrderString = orderString
            });
        }

        public void LogMessage(string message)
        {
            Log.Add(new OrderLogItem
            {
                User = "__SYSTEM__",
                Message = message
            });
        }

        public Pieces Pieces { get; set; }

        public string FileUrl { get; set; }

        public bool CanView(IPrincipal user)
        {
            if (user.IsInRole(AppRoles.Administrator)) return true;

            //check ownership
            if (OrderAmbassador != null)
                return OrderAmbassador.UserId == user.Identity.GetUserId() ||
                       OrderRequestor.UserId == user.Identity.GetUserId();
            return OrderRequestor.UserId == user.Identity.GetUserId();
        }

        private string StringToCSVCell(string str)
        {
            if (str == null)
                str = "";

            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }
                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }

        public override string ToString()
        {
            var separator = ",";

            List<string> listOrder = new List<string>
            {
                this.Id.ToString(),
                this.Status.ToString(),
                StringToCSVCell(this.Pieces.GetPercentage().ToString()),
                this.Date.ToString(),
                this.AmputationType.ToString(),
                this.ProductType.ToString(),
                this.Color.ToString(),
                //this.Extras.ToString(),
                this.Sizes?.Id.ToString(),
                this.Sizes?.A.ToString(),
                this.Sizes?.B.ToString(),
                this.Sizes?.C.ToString(),
                this.Sizes?.D.ToString(),
                StringToCSVCell(this.Comments),
                this.IdImage ?? null,
                this.ProofOfDelivery ?? null,
                this.DeliveryCourier.ToString(),
                StringToCSVCell(this.DeliveryTrackingCode),
                this.DeliveryPostalLabel ?? null,
                this.StatusLastUpdated.ToString(),

                this.OrderRequestor?.ToString(),
                this.OrderAmbassador?.ToString(),

            };

            return string.Join(separator, listOrder);
        }

        //TODO: Change this
        public List<String> GetTitles()
        {
            List<String> titles = new List<string>
            {
                "Pedido Nro",
                "Estado",
                "Porcentaje",
                "Creado",
                "Amputacion",
                "Producto",
                "Color",
                "Tamanio-Id",
                "Tamanio-A",
                "Tamanio-B",
                "Tamanio-C",
                "Tamanio-D",
                "Comentarios",
                "Foto",
                "Prueba de envio",
                "Courier",
                "Traking",
                "Etiqueta Postal",
                "Ultima Actualización de estado",
            };
            //var orderTitles = titles.Union(this.OrderRequestor.GetTitles().Union(this.OrderAmbassador?.GetTitles()));

            return titles;
        }

    }


    public class Pieces
    {
        public Pieces() { }

        public Pieces(bool all)
        {
            AtomicLabCover = all;
            FingerMechanismHolder = all;
            Fingers = all;
            FingerStopper = all;
            FingersX1 = all;
            FingersX2P = all;
            Palm = all;
            ThumbConnector = all;
            ThumbClip = all;
            Thumb = all;
            ThumbScrew = all;
            UpperArm_FingerConnector = all;
            UpperArm_PalmConnector = all;
            UpperArm_ThumbShortConnector = all;
            UpperArm_FingerSlider = all;
            UpperArm = all;
        }

        public bool AtomicLabCover { get; set; }
        public bool FingerMechanismHolder { get; set; }
        public bool Fingers { get; set; }
        public bool FingerStopper { get; set; }
        public bool FingersX1 { get; set; }
        public bool FingersX2P { get; set; }
        public bool Palm { get; set; }
        public bool ThumbConnector { get; set; }
        public bool Thumb { get; set; }
        public bool ThumbClip { get; set; }
        public bool ThumbScrew { get; set; }
        public bool UpperArm_FingerConnector { get; set; }
        public bool UpperArm_PalmConnector { get; set; }
        public bool UpperArm_ThumbShortConnector { get; set; }
        public bool UpperArm_FingerSlider { get; set; }
        public bool UpperArm { get; set; }

        public bool Ready()
        {
            return AtomicLabCover
                   && FingerMechanismHolder
                   && Fingers
                   && FingerStopper
                   && FingersX1
                   && FingersX2P
                   && Palm
                   && ThumbConnector
                   && ThumbClip
                   && Thumb
                   && ThumbScrew
                   && UpperArm_FingerConnector
                   && UpperArm_PalmConnector
                   && UpperArm_ThumbShortConnector
                   && UpperArm_FingerSlider
                   && UpperArm;
        }

        public bool AllSet()
        {
            return Ready();
        }

        public int GetPercentage()
        {
            var pCount = 0;
            var pTrue = 0;
            var t = typeof(Pieces);
            foreach (var prop in t.GetProperties())
            {
                if (typeof(bool) != prop.PropertyType) continue;
                pCount++;
                if ((bool) prop.GetValue(this, null))
                {
                    pTrue++;
                }
            }

            return (int)((float)pTrue / pCount * 100);
        }
    }

    public class OrderLogItem
    {
        public OrderLogItem()
        {
            Date = DateTime.UtcNow;
        }

        public DateTime Date { get; set; }

        public string User { get; set; }

        public string Message { get; set; }

        public string OrderString { get; set; }
    }

    public enum OrderStatus
    {
        [Description("No asignado")] NotAssigned = 0,
        [Description("Pre-asignado")] PreAssigned = 1,
        [Description("Imprimiendo")] Pending = 2,
        [Description("Lista")] Ready = 3,
        [Description("Entregada")] Delivered = 4,
        [Description("Coordinando envío")] ArrangeDelivery = 5,
        [Description("Rechazada")] Rejected = 6
    }

    public enum Courier
    {
        [Description("Sin envío (entrega personalmente)")] NoCourier = 0,
        [Description("Andreani")] Andreani = 1,
    }

    public enum OrderColor
    {
        [Description("Blanco y rojo")] A = 0,
        [Description("Azul y rojo")] B = 1,
        [Description("Rosa y blanco")] C = 2,
        [Description("Azul y amarillo")] D = 3,
        [Description("Azul blanco y rojo")] E = 4,
        [Description("Rojo y amarillo")] F = 5,
    }

    public enum ProductType
    {
        [Description("Derecha")] Right = 0,
        [Description("Izquierda")] Left = 1,
    }
}