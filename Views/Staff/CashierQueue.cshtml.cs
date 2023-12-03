using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Persol_HMS.Views.Staff
{
    public class CashierQueue : PageModel
    {
        private readonly ILogger<CashierQueue> _logger;

        public CashierQueue(ILogger<CashierQueue> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}