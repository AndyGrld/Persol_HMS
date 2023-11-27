using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Auth.Views.Shared
{
    public class Navbar : PageModel
    {
        private readonly ILogger<Navbar> _logger;

        public Navbar(ILogger<Navbar> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}