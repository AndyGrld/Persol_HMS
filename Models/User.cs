using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Persol_HMS.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "The First Name is required.")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required(ErrorMessage = "The Last Name is required.")]
        public string LastName { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Please select a department.")]
        public int DepartmentId { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "The Status is required.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "The Date of Birth is required.")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public bool LockEnabled { get; set; }

        public int Attempts { get; set; }

        [Display(Name = "Lock End Date")]
        [DataType(DataType.Date)]
        public DateTime? LockEnd { get; set; }

        // Navigation property for the department
        public virtual Department? Department { get; set; }
    }
}
