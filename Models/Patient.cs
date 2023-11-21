using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Persol_HMS.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        
        public string PatientNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? InsuranceType { get; set; }
        public string? InsuranceNo { get; set;}
        public char Gender { get; set; }
        public string EmergencyContactFirstName { get; set; }
        public string EmergencyContactLastName { get; set; }
        public string EmergencyContactNo { get; set; }
    }
}