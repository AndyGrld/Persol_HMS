using System;
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
        [Required(ErrorMessage = "Drug Name is required.")]
        public string DrugName { get; set; }

        [Required(ErrorMessage = "Dosage is required.")]
        public string Dosage { get; set; }

        public List<Drug> DrugNames { get; set; }

        public CreateMedicalViewModel(){
            var drugs = new List<Drug>();
            for (int i = 0; i < 10; i++)
            {
                drugs.Add(new Drug());
            }
            DrugNames = drugs;
        }
    }
}
