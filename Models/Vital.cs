using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persol_Hms.Models
{
    public class Vital
    {
        [Key] 
        public int Id { get; set; }
        [ForeignKey(nameof(Patient))]
        public int PatientNo { get; set; }
        public double Temperature { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string BloodPressure { get; set; }
        public DateTime Date { get; set; }
    }
}
