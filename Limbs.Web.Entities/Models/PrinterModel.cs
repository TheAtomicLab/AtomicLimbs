using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.Entities.Models
{
    public class PrinterModel
    {
        [Key]
        public int Id { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
        public int Long { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }   

        public string PrintingArea { get; set; }

        public bool IsHotBed { get; set; }
    }
}