using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Persol_HMS.Models;

namespace Persol_HMS.Models
{
    public class Patient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Patient Number")]
        [Required(ErrorMessage = "The Patient Number is required.")]

        [Key]
        public string? PatientNo { get; set; }

        [Required(ErrorMessage = "The First Name is required.")]

        public string FirstName { get; set; }

        [Required(ErrorMessage = "The Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The Contact Number is required.")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Invalid Contact Number.")]
        public string ContactNo { get; set; }

        public class Max130YearsOldAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                DateTime dateOfBirth = (DateTime)value;
                if (dateOfBirth > DateTime.Now || dateOfBirth < DateTime.Now.AddYears(-130))
                {
                    return false;
                }
                return true;
            }
        }

        [Required(ErrorMessage = "The Date of Birth is required.")]
        [Max130YearsOld(ErrorMessage = "The Date of Birth is invalid.")]
        [DataType(DataType.Date)]
        
        public DateTime DateOfBirth { get; set; }


        [Display(Name = "Insurance Type")]
        public string? InsuranceType { get; set; }

        [Display(Name = "Insurance Number")]
        public string? InsuranceNo { get; set; }

        [Required(ErrorMessage = "The Gender is required.")]
        [RegularExpression("^[MF]$", ErrorMessage = "Invalid Gender.")]
        public char Gender { get; set; }

        [Display(Name = "Emergency Contact First Name")]
        [Required(ErrorMessage = "The Emergency Contact First Name is required.")]
        public string EmergencyContactFirstName { get; set; }

        [Display(Name = "Emergency Contact Last Name")]
        [Required(ErrorMessage = "The Emergency Contact Last Name is required.")]
        public string EmergencyContactLastName { get; set; }

        [Display(Name = "Emergency Contact Number")]
        [Required(ErrorMessage = "The Emergency Contact Number is required.")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Invalid Emergency Contact Number.")]
        public string EmergencyContactNo { get; set; }

        [InverseProperty("Patient")]
        public ICollection<Medical> Medicals { get; set; }
    }
}
