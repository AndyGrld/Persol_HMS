using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Persol_HMS.Views.Staff
{
    public class DoctorQueue : PageModel
    {
        private readonly ILogger<DoctorQueue> _logger;

        public DoctorQueue(ILogger<DoctorQueue> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}