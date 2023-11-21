using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persol_Hms.Models
{
    public class Queue
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Patient))]
        public int PatientNo { get; set; }
        public int QueueNo { get; set; }
        public string Status { get; set; }
        public DateTime DateToday { get; set; }
    }

}
