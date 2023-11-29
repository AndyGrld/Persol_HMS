using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persol_HMS.Data;
using Persol_HMS.Models;

[Authorize]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    private int? GetDepartmentId()
    {
       var user = _context.Users.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name));
       return user?.DepartmentId;
    }

    private IActionResult RedirectToHome()
    {
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Index()
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 5)
        {
            return RedirectToHome();
        }
        var users = _context.Users.Include(u => u.Department).ToList();
        return View(users);
    }

    public IActionResult Details(string id)
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 5)
        {
            return RedirectToHome();
        }
        if (id == null)
        {
            return NotFound();
        }

        var user = _context.Users
            .Include(u => u.Department)
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    public IActionResult Edit(string id)
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 5)
        {
            return RedirectToHome();
        }
        if (id == null)
        {
            return NotFound();
        }

        var user = _context.Users.Find(id);
        if (user == null)
        {
            return NotFound();
        }

        // You might want to load the departments for a dropdown list in your view
        ViewBag.Departments = _context.Departments.ToList();

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, User user)
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 5)
        {
            return RedirectToHome();
        }
        if (id != user.Id)
        {
            return NotFound();
        }

        if (user.UserName != null)
        {
            try
            {
                _context.Update(user);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        return View(user);
    }

    public IActionResult Delete(string id)
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 5)
        {
            return RedirectToHome();
        }
        var user = _context.Users.Find(id);
        _context.Users.Remove(user);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    private bool UserExists(string id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}
