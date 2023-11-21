using System.ComponentModel.DataAnnotations;

namespace Persol_HMS.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int DepartmentID { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool LockEnabled { get; set; }
        public int Attempts { get; set; }
        public DateTime LockEnd { get; set; }


    }
}