namespace Persol_Hms.Models
{
    public class Drugs
    {


        public int ID { get; set; }
        public int PatientID { get; set; }
        public string DrugName { get; set; }
        public string Dosage { get; set; }
        public DateTime Date { get; set; }


    }
}
