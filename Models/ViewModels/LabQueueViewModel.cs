using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persol_HMS.Models.ViewModels
{
    public class LabQueueViewModel
    {
        public List<Lab> Labs { get; set; }
        public QueueViewModel QueueViewModel { get; set; }

        public List<Lab> LabNames { get; set; }
        public List<string> LabResult { get; set; }
        public List<string> LabNotes { get; set; }
    }
}