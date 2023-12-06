using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persol_HMS.Models
{
    public class Medical
    {
        [Key]
        public int ID { get; set; }//
        public DateTime Date { get; set; }//
        [ForeignKey(nameof(Vital))]
        public int? VitalsID { get; set; }//
        [ForeignKey(nameof(Symptom))]
        public int SymptomsID { get; set; }//
        public string Diagnoses { get; set; }//
        public string? WardName { get; set; }//
        [DataType(DataType.Currency)]
        public double Bill { get; set; } = 0;
        public bool isPaid { get; set; } = false;
        public bool IsAdmitted { get; set; }//
        public DateTime? DateAdmitted { get; set; }//
        [ForeignKey(nameof(Patient))]
        public string PatientNo { get; set; }//
        
        public virtual Vital Vital { get; set; }//
        public virtual List<Drug> Drugs { get; set; }//
        public virtual Symptom Symptom { get; set; }//
        public virtual List<Lab> Labs { get; set; }
        public virtual Patient Patient { get; set; }//
        public virtual Queue Queue { get; set; }
    }
}
