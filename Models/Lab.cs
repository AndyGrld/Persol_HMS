using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persol_Hms.Models
{
    public class Lab
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey(nameof(Patient))]
        public int PatientNo { get; set; }
        public string LabName { get; set; }
        public string Result { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }
    }

}
