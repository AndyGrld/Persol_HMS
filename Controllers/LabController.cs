using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Mvc;
using Persol_HMS.Data;
using Persol_HMS.Models;
using System;
using System.Linq;

namespace Persol_HMS.Controllers
{
    public class LabController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LabController(ApplicationDbContext context)
        {
            _context = context;
        }

        
    }
}
