using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Auth.Views.Shared
{
    public class SidebarPartial : PageModel
    {
        private readonly ILogger<SidebarPartial> _logger;

        public SidebarPartial(ILogger<SidebarPartial> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}