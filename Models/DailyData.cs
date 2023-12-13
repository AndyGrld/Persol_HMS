using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Persol_HMS.Models
{
    public class DailyData
    {
        [Key]
        public int ID { get; set; }
        public int TotalPatients { get; set; }
        public int TotalAdmitted { get; set; }
        public int TotalDischarged { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
    }
}