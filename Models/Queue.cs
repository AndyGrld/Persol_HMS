namespace Persol_Hms.Models
{
    public class Queue
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int QueueNo { get; set; }
        public string Status { get; set; }
        public DateTime DateToday { get; set; }
    }

}
