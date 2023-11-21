using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persol_HMS.Models
{
    public class Symptom
    {
        [Key]
        public int ID { get; set; }
        public string Symptoms { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey(nameof(Patient))]
        public string PatientNo { get; set; }

    }
}
