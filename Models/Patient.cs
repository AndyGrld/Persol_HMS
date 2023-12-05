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
        [Required(ErrorMessage = "Please enter Patient Number")]

        [Key]
        public string? PatientNo { get; set; }

        [Required(ErrorMessage = "Please enter First Name")]

        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter Contact Number")]
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

        [Required(ErrorMessage = "Please enter Date of Birth")]
        [Max130YearsOld(ErrorMessage = "Please enter valid Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }


        [Display(Name = "Insurance Type")]
        [Required(ErrorMessage = "Please enter Insurance Provider")]
        public string? InsuranceType { get; set; }

        [Display(Name = "Insurance Number")]
        [Required(ErrorMessage = "Please enter Insurance Number")]
        public string? InsuranceNo { get; set; }

        // [Display(Name = "Insurance Expiry Date")]
        // [Required(ErrorMessage = "Please enter date Insurance Expires")]
        // public DateTime InsuranceExpireDate { get; set; }

        [Required(ErrorMessage = "Please enter Gender")]
        [RegularExpression("^[MF]$", ErrorMessage = "Invalid Gender.")]
        public char Gender { get; set; }

        [Display(Name = "Emergency Contact First Name")]
        [Required(ErrorMessage = "Please enter Emergency Contact First Name")]
        public string EmergencyContactFirstName { get; set; }

        [Display(Name = "Emergency Contact Last Name")]
        [Required(ErrorMessage = "Please enter Emergency Contact Last Name")]
        public string EmergencyContactLastName { get; set; }

        [Display(Name = "Emergency Contact Number")]
        [Required(ErrorMessage = "Please enter Emergency Contact Number")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Invalid Emergency Contact Number.")]
        public string EmergencyContactNo { get; set; }

        [InverseProperty("Patient")]
        public ICollection<Medical> Medicals { get; set; }
    }
}
