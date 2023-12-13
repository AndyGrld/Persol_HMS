using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persol_HMS.Models.ViewModels
{
    public class PatientListView
    {
		public int TotalPatients { get; set; }
		public int PageSize { get; set; }
		public int CurrentPage { get; set; }
		public string? Search { get; set; }
		public List<Patient>? Patients { get; set; }
    }
}