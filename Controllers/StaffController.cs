using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Persol_HMS.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Doctor(string username)
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