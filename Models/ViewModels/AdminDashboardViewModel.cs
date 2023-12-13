using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persol_HMS.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public virtual List<DailyData> DailyDatas { get; set; }
        public virtual List<User> Records { get; set; }
        public virtual List<User> Nurses { get; set; }
        public virtual List<User> Doctors { get; set; }
        public virtual List<User> LabPersonnels { get; set; }
        public virtual List<User> Pharmacists { get; set; }
        public virtual List<Patient> Patients { get; set; }
        public virtual List<User> Cashiers { get; set; }
        public virtual List<User> Admins { get; set; }
        public int TotalUsers { get; set; }
    }
}