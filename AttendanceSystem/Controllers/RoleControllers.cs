using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Split these into separate files under /Controllers when you add them.
namespace AttendanceSystem.Controllers
{
    // ---------- Controllers/HomeController.cs ----------
    public class HomeController : Controller
    {
        // After login, send each user to their own dashboard
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))   return RedirectToAction("Index", "Admin");
                if (User.IsInRole("Teacher")) return RedirectToAction("Index", "Teacher");
                if (User.IsInRole("Student")) return RedirectToAction("Index", "Student");
            }
            return View();   // public landing page
        }

        public IActionResult Error() => View();
    }

    // ---------- Controllers/AdminController.cs ----------
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index() => View();
    }

    // ---------- Controllers/TeacherController.cs ----------
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        public IActionResult Index() => View();
    }

    // ---------- Controllers/StudentController.cs ----------
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        public IActionResult Index() => View();
    }
}

// For each controller add a simple view, e.g. Views/Admin/Index.cshtml:
//
// @{ ViewData["Title"] = "Admin Dashboard"; }
// <h2>Admin Dashboard</h2>
// <p>Welcome, @User.Identity?.Name</p>