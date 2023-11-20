using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Persol_HMS.Views.Staff
{
    public class Nurse : PageModel
    {
        private readonly ILogger<Nurse> _logger;

        public Nurse(ILogger<Nurse> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}