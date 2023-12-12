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
        public int PatientsToday { get; set; }
        public int AdmittedToday { get; set; }
        public int DischargedToday { get; set; }
    }
}