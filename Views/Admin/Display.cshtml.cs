using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Persol_HMS.Views.Admin
{
    public class Display : PageModel
    {
        private readonly ILogger<Display> _logger;

        public Display(ILogger<Display> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}