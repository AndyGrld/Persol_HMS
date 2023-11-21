using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persol_Hms.Models
{
    public class Symptom
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey(nameof(Patient))]
        public int PatientNo { get; set; }
        public string Symptoms { get; set; }
        public DateTime Date { get; set; }  
    }
}
