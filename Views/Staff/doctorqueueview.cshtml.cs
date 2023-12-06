using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Persol_HMS.Views.Staff
{
    public class doctorqueueview : PageModel
    {
        private readonly ILogger<doctorqueueview> _logger;

        public doctorqueueview(ILogger<doctorqueueview> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}