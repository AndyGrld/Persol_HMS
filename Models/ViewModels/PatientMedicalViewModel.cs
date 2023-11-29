using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persol_HMS.Models.ViewModels
{
    public class PatientMedicalViewModel
    {
        public Patient Patient { get; set; }
        public List<MedicalViewModel> MedicalRecords { get; set; }
    }

    public class MedicalViewModel
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public Vital Vital { get; set; }
        public string Diagnoses { get; set; }
        public Symptom Symptom { get; set; }
        public List<Drug> Drugs { get; set; }
        public List<Lab> Labs { get; set; }
    }
}