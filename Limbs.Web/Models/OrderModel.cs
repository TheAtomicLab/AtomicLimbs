using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Limbs.Web.Models
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Person who request the order
        /// </summary>
        public UserModel OrderRequestor { get; set; }

        /// <summary>
        /// Person who will use the order
        /// </summary>
        public virtual UserModel OrderUser { get; set; }

        public int Design { get; set; }

        public virtual ICollection<AccessoryModel> Extras { get; set; }
        
        public virtual OrderSizesModel Sizes { get; set; }

        public string Comments { get; set; }

        public OrderStatus Status { get; set; }
    }

    public enum OrderStatus
    {
        [Description("Pendiente")]
        Pending,
        [Description("Lista para retirar")]
        Ready,
        [Description("Entregada")]
        Delivered,
    }
    
    public static class AttributesHelperExtension
    {
        public static string ToDescription(this Enum value)
        {
            var da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return da.Length > 0 ? da[0].Description : value.ToString();
        }
    }

    
}