using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persol_HMS.Models
{
    public class Vital
    {
        [Key] 
        public int Id { get; set; }
        [ForeignKey(nameof(Patient))]
        public string PatientNo { get; set; }
        [Required(ErrorMessage = "Please enter Temperature")]
        public double Temperature { get; set; }
        [Required(ErrorMessage = "Please enter Height")]
        public double Height { get; set; }
        [Required(ErrorMessage = "Please enter Weight")]
        public double Weight { get; set; }
        [Required(ErrorMessage = "Please enter Blood Pressure")]
        public string BloodPressure { get; set; }
        public DateTime Date { get; set; }
    }
}
