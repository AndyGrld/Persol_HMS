using System;
using System.ComponentModel.DataAnnotations;

namespace Persol_HMS.Models
{
    public enum DepartmentType
    {
        Doctor,
        Nurse,
        Lab,
        RecordClerk
    }

    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        [Display(Name = "Department Code")]
        public int DepartmentCode { get; set; }
    }
}
