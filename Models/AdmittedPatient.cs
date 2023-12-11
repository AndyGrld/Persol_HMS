using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Persol_HMS.Models
{
    public class AdmittedPatient
    {
        [Key]
        public int ID { get; set; }
        public string PatientNo { get; set; }
        [ForeignKey(nameof(Medical))]
        public int MedicalID { get; set; }
        public virtual Medical Medical { get; set; }

        public string PatientName { get; set; }

        public DateTime DateAdmitted { get; set; }
        public string WardName { get; set; }
    }
}