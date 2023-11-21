namespace Persol_Hms.Models
{
    public class Symptoms
    {
        public int ID { get; set; }
        public int PatientID { get; set; }
        public string Symptom { get; set; }
        public DateTime Date { get; set; }  
    }
}
