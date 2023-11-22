using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persol_HMS.Models
{
    public class Medical
    {
        [Key]
        public int ID { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey(nameof(Vital))]
        public int VitalsID { get; set; }
        [ForeignKey(nameof(Drug))]
        public int DrugsID { get; set; }
        [ForeignKey(nameof(Symptom))]
        public int SymptomsID { get; set; }
        [ForeignKey(nameof(Lab))]
        public int LabID { get; set; }
        public string Diagnoses { get; set; }
        public int WardNo { get; set; }
        public bool IsAdmitted { get; set; }
        public DateTime DateAdmitted { get; set; }
        [ForeignKey(nameof(Patient))]
        public string PatientNo { get; set; }
        
        public virtual Vital Vital { get; set; }
        public virtual Drug Drug { get; set; }
        public virtual Symptom Symptom { get; set; }
        public virtual Lab Lab { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
