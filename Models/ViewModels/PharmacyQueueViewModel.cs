using System.Collections.Generic;

namespace Persol_HMS.Models.ViewModels
{
    public class PharmacyQueueViewModel
    {
        public List<Queue> PatientsInLine { get; set; }
        public List<PatientWithDrugs> PatientsWithDrugs { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPatients { get; set; }
        public string Search { get; set; }
    }

    public class PatientWithDrugs
    {
        public Queue PatientQueue { get; set; }
        public List<Drug> PatientDrugs { get; set; }
    }
}
