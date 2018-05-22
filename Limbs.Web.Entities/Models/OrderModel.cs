using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

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
        [Display(Name = "Pedido #", Description = "")]
        public int Id { get; set; }

        [Display(Name = "Creado", Description = "")]
        public DateTime Date { get; set; }

        /// <summary>
        ///     Usuario que solicita la orden de protesis
        /// </summary>
        public UserModel OrderRequestor { get; set; }

        /// <summary>
        ///     Embajador que procesara la orden
        /// </summary>
        public virtual AmbassadorModel OrderAmbassador { get; set; }

        [Display(Name = "Color", Description = "(si es posible)")]
        public OrderColor Color { get; set; }

        [Display(Name = "¿Cuál?", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public ProductType ProductType { get; set; }

        public virtual ICollection<AccessoryModel> Extras { get; set; }

        [NotMapped]
        [Display(Name = "Tamaños", Description = "")]
        public virtual OrderSizesModel Sizes { get; set; }

        public string SizesData
        {
            get => JsonConvert.SerializeObject(Sizes);
            set => Sizes = value != null
                ? JsonConvert.DeserializeObject<OrderSizesModel>(value)
                : new OrderSizesModel();
        }

        [Display(Name = "Amputación", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public AmputationType AmputationType { get; set; }

        [Display(Name = "Comentarios", Description = "")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [Display(Name = "Foto", Description = "")]
        public string IdImage { get; set; }

        [Display(Name = "Estado", Description = "")]
        public OrderStatus Status { get; set; }

        [Display(Name = "Última modificación", Description = "")]
        public DateTime StatusLastUpdated { get; set; }

        [Display(Name = "Prueba de entrega", Description = "")]
        public string ProofOfDelivery { get; set; }

        [Display(Name = "Courier", Description = "")]
        public Courier DeliveryCourier { get; set; }

        [Display(Name = "Tracking", Description = "")]
        public string DeliveryTrackingCode { get; set; }

        [Display(Name = "Etiqueta postal", Description = "")]
        public string DeliveryPostalLabel { get; set; }

        [NotMapped]
        [Display(Name = "Historial de cambios", Description = "")]
        public List<OrderLogItem> Log { get; private set; }

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

        public override string ToString()
        {
            var separator = ",";

            List<String> listOrder = new List<String>
            {
                this.Id.ToString(),
                this.Status.ToString(),
                String.Concat("\"",this.Pieces.GetPercentage().ToString(),"\""),
                this.Date.ToString(),
                this.AmputationType.ToString(),
                this.ProductType.ToString(),
                this.Color.ToString(),
                //this.Extras.ToString(),
                this.Sizes?.ToString(),
                this.SizesData.ToString(),
                String.Concat("\"",this.Comments,"\""),
                this.IdImage,
                this.ProofOfDelivery,
                this.DeliveryCourier.ToString(),
                this.DeliveryTrackingCode,
                this.DeliveryPostalLabel,
                this.StatusLastUpdated.ToString(),

                this.OrderRequestor.ToString(),
                this.OrderAmbassador?.ToString(),

            };

            return String.Join(separator, listOrder);
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
    }

    public enum OrderStatus
    {
        [Description("No asignado")] NotAssigned = 0,
        [Description("Pre-asignado")] PreAssigned = 1,
        [Description("Imprimiendo")] Pending = 2,
        [Description("Lista")] Ready = 3,
        [Description("Entregada")] Delivered = 4,
        [Description("Coordinando envío")] ArrangeDelivery = 5,
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