using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persol_Hms.Models
{
    public class Drug
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey(nameof(Patient))]
        public string PatientNo { get; set; }
        public string DrugName { get; set; }
        public string Dosage { get; set; }
        public DateTime Date { get; set; }


    }
}
