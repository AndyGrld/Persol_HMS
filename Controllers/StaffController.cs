using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Persol_HMS.Controllers
{
    public class StaffController : Controller
    {
        [HttpGet]
        public IActionResult Doctor()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RecordsClerk()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Nurse()
        {
            return View();
        }
    }
}