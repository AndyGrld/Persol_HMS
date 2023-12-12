using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Persol_HMS.Models
{
    public class Revenue
    {
        [Key]
        public int ID { get; set; }
        public double Amount { get; set; }
        public DateTime Period { get; set; }
    }
}