using System.ComponentModel.DataAnnotations;

namespace Persol_Hms.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentCode { get; set; }
    }

}
