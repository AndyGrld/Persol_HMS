using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Persol_HMS.Views.Staff
{
    public class NurseQueue : PageModel
    {
        private readonly ILogger<NurseQueue> _logger;

        public NurseQueue(ILogger<NurseQueue> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}