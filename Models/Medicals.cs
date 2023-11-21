namespace Persol_Hms.Models
{
    public class Medicals
    {


        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int VitalsID { get; set; }
        public int DrugsID { get; set; }
        public int SymptomsID { get; set; }
        public string Diagnoses { get; set; }
        public int LabID { get; set; }
        public int WardNo { get; set; }
        public bool IsAdmitted { get; set; }
        public DateTime DateAdmitted { get; set; }


    }
}
