using System.Collections.Generic;
using Persol_HMS.Models;

namespace Persol_HMS.Models.ViewModels
{
    public class PharmacyQueueViewModel
    {
        public QueueViewModel QueueViewModel { get; set; }
        public Patient Patient { get; set; }
        public List<Medical> MedicalRecords { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
