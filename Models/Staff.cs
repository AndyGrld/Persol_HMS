using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Persol_HMS.Models
{
    public class Staff : IdentityUser
    {
        public string? Role { get; set; }
    }
}