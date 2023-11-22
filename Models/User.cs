using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Persol_HMS.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int DepartmentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool LockEnabled { get; set; }
        public int Attempts { get; set; }
        public DateTime? LockEnd { get; set; }

        // Navigation property for the department
        public Department Department { get; set; }
    }
}
