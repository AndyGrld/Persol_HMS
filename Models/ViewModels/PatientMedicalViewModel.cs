using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persol_HMS.Models.ViewModels
{
    public class PatientMedicalViewModel
    {
        public Patient Patient { get; set; }
        public List<Year> YearlyRecords { get; set; }
    }

    public class MedicalList
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public Vital Vital { get; set; }
        public string Diagnoses { get; set; }
        public Symptom Symptom { get; set; }
        public List<Drug> Drugs { get; set; }
        public List<Lab> Labs { get; set; }
    }

    public class Year
    {
        public int YearNo { get; set; }
        public List<Month> Months { get; set; }
    }

    public class Month
    {
        public int MonthNo { get; set; }
		public DateTime Date { get; set; }
        public List<MedicalList> MedicalRecords { get; set; }
    }
}