using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persol_HMS.Models.ViewModels
{
	public class AdmittedViewModel{
		public int TotalPatients { get; set; }
		public int PageSize { get; set; }
		public int CurrentPage { get; set; }
		public string? Search { get; set; }
		public List<AdmittedPatient>? Patients { get; set; }
	}
}