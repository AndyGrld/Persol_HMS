using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persol_HMS.Models.ViewModels
{
    public class CashierQueueViewModel
    {
        public QueueViewModel QueueViewModel { get; set; }
        public List<PatientWithLatestMedical> PatientsWithLatestMedical { get; set; }
    }

    public class PatientWithLatestMedical{
        public Medical latestMedical { get; set; }
        public Queue PatientQueue { get; set; }
    }
}