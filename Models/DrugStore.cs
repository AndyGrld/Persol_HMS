using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Persol_HMS.Models
{
    public class DrugStore
    {
        [Key]
        public int Id { get; set; }
        public string DrugName { get; set; }
        public int Type { get; set; } // 1 => Tablet, 0 => Syrup
        public double Price { get; set; } = 0;
        public int Quantity { get; set; } = 100;
    }
}
