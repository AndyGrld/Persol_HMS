namespace Persol_Hms.Models
{
    public class Vitals
    {
        public int VitalsID { get; set; }
        public int PatientId { get; set; }
        public double Temperature { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string BloodPressure { get; set; }
        public DateTime Date { get; set; }
    }
}
