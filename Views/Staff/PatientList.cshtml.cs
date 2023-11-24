using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Persol_HMS.Views.Staff
{
    public class PatientList : PageModel
    {
        private readonly ILogger<PatientList> _logger;

        public PatientList(ILogger<PatientList> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}