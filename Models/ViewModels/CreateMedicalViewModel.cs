using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Persol_HMS.Models.ViewModels
{
    public class CreateMedicalViewModel
    {
        // Fields for patient information
        public string PatientNo { get; set; }

        // Fields for medical records
        [Required(ErrorMessage = "Symptoms is required.")]
        public string Symptoms { get; set; }

        [Required(ErrorMessage = "Diagnoses is required.")]
        public string Diagnoses { get; set; }

        [Required(ErrorMessage = "Ward Number is required.")]
        public int? WardNo { get; set; }

        [Display(Name = "Admitted")]
        public bool IsAdmitted { get; set; }

        [Display(Name = "Date Admitted")]
        [DataType(DataType.Date)]
        public DateTime? DateAdmitted { get; set; }

        // Fields for drug information
        public List<Drug> DrugNames { get; set; }
        public List<Medical> MedicalRecords { get; set; }
        public List<Medical> ExistingMedicalRecords { get; set; }
        public List<Queue> NewPatients { get; set; }

        // Fields for lab information
        [Display(Name = "Needs Lab")]
        public bool NeedsLab { get; set; }

        public List<string> WardNames { get; set; }
        public List<string> LabNames { get; set; }

        // New property for selected lab names
        public List<string> SelectedLabNames { get; set; }

        // New property for selected ward names
        public List<string> SelectedWardNames { get; set; }

        // Add the new property for available lab names
        public List<string> AvailableWardNames { get; set; }
        public string LabName { get; set; }

        public string Dosage { get; set; }

        // Add the new property for available lab names
        public List<string> AvailableLabNames { get; set; }

        public CreateMedicalViewModel()
        {
            AvailableLabNames = new List<string>
            {
                "Blood Test",
                "Urinalysis",
                "X-ray",
                "MRI",
                "CT Scan",
                // Add more lab names as needed
            };
            LabNames = new List<string>();
            SelectedLabNames = new List<string>();

            AvailableWardNames = new List<string>
            {
                "Pediatrics Ward",
                "Medical Ward",
                "Surgical Ward",
                "Maternity Ward",
                "Psychiatric Ward",
                "Intensive Care Unit",
                "Isolation Ward"
            };
            SelectedWardNames = new List<string>();
            WardNames = new List<string>();
        }
    }
}
