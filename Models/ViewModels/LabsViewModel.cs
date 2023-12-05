using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persol_HMS.Models.ViewModels
{
    public class LabsViewModel
    {
        public Patient patient { get; set; }
        public List<Lab> Labs { get; set; }
    }
}
