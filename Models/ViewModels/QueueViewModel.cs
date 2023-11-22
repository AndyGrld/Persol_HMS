using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Persol_HMS.Models.ViewModels
{
    public class QueueViewModel
    {
        public List<Queue> PatientsInLine { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPatients { get; set; }

        public string Search { get; set; }
    }
}
