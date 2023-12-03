using System.Collections.Generic;

namespace Persol_HMS.Models.ViewModels
{
    public class PharmacyQueueViewModel
    {
        public QueueViewModel QueueViewModel { get; set; }
        public List<PatientWithDrugs> PatientsWithDrugs { get; set; }
    }

    public class PatientWithDrugs
    {
        public Queue PatientQueue { get; set; }
        public List<Drug> PatientDrugs { get; set; }
    }
}
