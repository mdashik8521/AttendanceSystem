using AttendanceSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AttendanceSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");

                if (User.IsInRole("Teacher"))
                    return RedirectToAction("Index", "Teacher");

                if (User.IsInRole("Student"))
                    return RedirectToAction("Index", "Student");
            }

            return View();
        }

        // =========================
        // ADMIN - TEACHERS LIST
        // =========================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Teachers()
        {
            var teachers = await _context.Teachers
                .Include(t => t.User)
                .Include(t => t.Department)
                .ToListAsync();

            return View(teachers);
        }

        // =========================
        // ADMIN - STUDENTS LIST
        // =========================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Students()
        {
            var students = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Department)
                .ToListAsync();

            return View(students);
        }

        public IActionResult Error()
        {
            return View();
        }
    }


    // =========================================================
    // ADMIN CONTROLLER
    // =========================================================

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalStudents = await _context.Students.CountAsync();
            ViewBag.TotalTeachers = await _context.Teachers.CountAsync();
            ViewBag.TotalDepartments = await _context.Departments.CountAsync();
            ViewBag.TotalSubjects = await _context.Subjects.CountAsync();
            ViewBag.TotalSessions = await _context.ClassSessions.CountAsync();
            ViewBag.TotalAttendanceRecords =
                await _context.AttendanceRecords.CountAsync();

            ViewBag.AcademicYear = "2026-2027";

            return View();
        }

        public async Task<IActionResult> Departments()
        {
            var departments = await _context.Departments
                .Include(d => d.Students)
                    .ThenInclude(s => s.User)
                .Include(d => d.Teachers)
                    .ThenInclude(t => t.User)
                .OrderBy(d => d.Name)
                .ToListAsync();

            return View(departments);
        }
                public async Task<IActionResult> Subjects()
        {
            var subjects = await _context.Subjects
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View(subjects);
        }

        
    }
    


    // =========================================================
    // TEACHER CONTROLLER
    // =========================================================

    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly AppDbContext _context;

        public TeacherController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var teacher = await _context.Teachers
                .Include(t => t.User)
                .Include(t => t.Department)
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound("Teacher profile not found.");
            }

            var subjectIds = teacher.Subjects
                .Select(s => s.Id)
                .ToList();

            var totalSessions = await _context.ClassSessions
                .CountAsync(cs => subjectIds.Contains(cs.SubjectId));

            var totalAttendanceRecords = await _context.AttendanceRecords
                .CountAsync(ar =>
                    subjectIds.Contains(ar.ClassSession!.SubjectId));

            // SQLite TimeSpan ordering fix
            var recentSessions = (await _context.ClassSessions
                .Include(cs => cs.Subject)
                .Where(cs => subjectIds.Contains(cs.SubjectId))
                .OrderByDescending(cs => cs.Date)
                .ToListAsync())
                .OrderByDescending(cs => cs.Date)
                .ThenByDescending(cs => cs.StartTime)
                .Take(5)
                .ToList();

            ViewBag.Teacher = teacher;
            ViewBag.TotalSubjects = teacher.Subjects.Count;
            ViewBag.TotalSessions = totalSessions;
            ViewBag.TotalAttendanceRecords = totalAttendanceRecords;
            ViewBag.RecentSessions = recentSessions;

            return View();
        }
    }


    // =========================================================
    // STUDENT CONTROLLER
    // =========================================================

    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return NotFound("Student profile not found.");
            }

            var attendanceRecords = await _context.AttendanceRecords
                .Include(a => a.ClassSession)
                    .ThenInclude(cs => cs!.Subject)
                .Where(a => a.StudentId == student.Id)
                .OrderByDescending(a => a.MarkedAt)
                .ToListAsync();

            var total = attendanceRecords.Count;

            var present = attendanceRecords.Count(a =>
                a.Status == AttendanceSystem.Models.AttendanceStatus.Present);

            var absent = attendanceRecords.Count(a =>
                a.Status == AttendanceSystem.Models.AttendanceStatus.Absent);

            var late = attendanceRecords.Count(a =>
                a.Status == AttendanceSystem.Models.AttendanceStatus.Late);

            var percentage = total == 0
                ? 0
                : Math.Round((double)present / total * 100, 1);

            ViewBag.Student = student;
            ViewBag.TotalAttendance = total;
            ViewBag.Present = present;
            ViewBag.Absent = absent;
            ViewBag.Late = late;
            ViewBag.AttendancePercentage = percentage;

            return View(attendanceRecords);
        }


        // =========================
        // STUDENT ATTENDANCE
        // =========================

        public async Task<IActionResult> Attendance()
        {
            var userId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return NotFound("Student profile not found.");
            }

            // SQLite TimeSpan ordering fix
            var records = (await _context.AttendanceRecords
                .Include(a => a.ClassSession)
                    .ThenInclude(cs => cs!.Subject)
                .Where(a => a.StudentId == student.Id)
                .OrderByDescending(a => a.ClassSession!.Date)
                .ToListAsync())
                .OrderByDescending(a => a.ClassSession!.Date)
                .ThenByDescending(a => a.ClassSession!.StartTime)
                .ToList();

            return View(records);
        }
    }
}