using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Persol_HMS.Views.Staff
{
    public class Lab : PageModel
    {
        private readonly ILogger<Lab> _logger;

        public Lab(ILogger<Lab> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}