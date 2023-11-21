namespace Persol_Hms.Models
{
    public class Lab
    {
        public int ID { get; set; }
        public int PatientID { get; set; }
        public string LabName { get; set; }
        public string Result { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }
    }

}
