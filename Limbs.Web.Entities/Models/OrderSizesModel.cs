using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.Entities.Models
{
    public class OrderSizesModel
    {
        public OrderSizesModel()
        {
            A = 0;
            B = 0;
            C = 0;
            D = 0;
        }

        [Key]
        public int Id { get; set; }

        public float A { get; set; }

        public float B { get; set; }

        public float C { get; set; }

        public int D { get; set; }
    }

}