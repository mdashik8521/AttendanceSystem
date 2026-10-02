using AttendanceSystem.Data;
using AttendanceSystem.Models;
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
        .Include(s => s.Teacher)
            .ThenInclude(t => t!.Department)
        .OrderBy(s => s.Name)
        .ToListAsync();

    ViewBag.Departments = await _context.Departments
        .OrderBy(d => d.Name)
        .ToListAsync();

    ViewBag.Teachers = await _context.Teachers
        .Include(t => t.User)
        .Include(t => t.Department)
        .OrderBy(t => t.User!.FullName)
        .ToListAsync();

    return View(subjects);
}



// 👇 YAHAN AddSubject paste karo

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AddSubject(
    string name,
    string code,
    int semester,
    int teacherId)
{
    if (await _context.Subjects.CountAsync() >= 6)
    {
        TempData["Error"] = "Maximum 6 subjects are allowed.";
        return RedirectToAction(nameof(Subjects));
    }

    if (string.IsNullOrWhiteSpace(name))
    {
        TempData["Error"] = "Subject name is required.";
        return RedirectToAction(nameof(Subjects));
    }

    if (string.IsNullOrWhiteSpace(code))
    {
        TempData["Error"] = "Subject code is required.";
        return RedirectToAction(nameof(Subjects));
    }

    if (await _context.Subjects.AnyAsync(s => s.Code == code))
    {
        TempData["Error"] = "A subject with this code already exists.";
        return RedirectToAction(nameof(Subjects));
    }

    var teacher = await _context.Teachers
        .FirstOrDefaultAsync(t => t.Id == teacherId);

    if (teacher == null)
    {
        TempData["Error"] = "Please select a valid teacher.";
        return RedirectToAction(nameof(Subjects));
    }

    var subject = new Subject
    {
        Name = name.Trim(),
        Code = code.Trim(),
        Semester = semester,
        TeacherId = teacherId
    };

    _context.Subjects.Add(subject);
    await _context.SaveChangesAsync();

    TempData["Success"] = "Subject added successfully.";

    return RedirectToAction(nameof(Subjects));
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteSubject(int id)
{
    var subject = await _context.Subjects
        .Include(s => s.Sessions)
        .FirstOrDefaultAsync(s => s.Id == id);

    if (subject == null)
    {
        TempData["Error"] = "Subject not found.";
        return RedirectToAction(nameof(Subjects));
    }

    if (subject.Sessions.Any())
    {
        TempData["Error"] =
            "This subject cannot be deleted because it has linked class sessions.";

        return RedirectToAction(nameof(Subjects));
    }

    _context.Subjects.Remove(subject);

    await _context.SaveChangesAsync();

    TempData["Success"] = "Subject deleted successfully.";

    return RedirectToAction(nameof(Subjects));
}

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

    // =========================
    // TEACHER DASHBOARD
    // =========================

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


    // =========================
    // TAKE ATTENDANCE - PAGE
    // =========================

    public async Task<IActionResult> TakeAttendance()
    {
        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var teacher = await _context.Teachers
            .Include(t => t.Subjects)
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (teacher == null)
        {
            return NotFound("Teacher profile not found.");
        }

        return View(
            teacher.Subjects
                .OrderBy(s => s.Name)
                .ToList()
        );
    }


    // =========================
    // CREATE CLASS SESSION
    // =========================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartSession(int subjectId)
    {
        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var teacher = await _context.Teachers
            .Include(t => t.Subjects)
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (teacher == null)
        {
            return NotFound("Teacher profile not found.");
        }

        // Make sure subject belongs to this teacher
        var subject = teacher.Subjects
            .FirstOrDefault(s => s.Id == subjectId);

        if (subject == null)
        {
            return Unauthorized();
        }

        // Generate unique QR token
        var token = Guid.NewGuid().ToString("N");

        var session = new ClassSession
        {
            SubjectId = subjectId,
            Date = DateTime.Today,
            StartTime = DateTime.Now.TimeOfDay,
            QrToken = token,

            // QR valid for 5 minutes
            QrExpiresAt = DateTime.Now.AddMinutes(5)
        };

        _context.ClassSessions.Add(session);

        await _context.SaveChangesAsync();

        return RedirectToAction(
            nameof(ShowQRCode),
            new { id = session.Id }
        );
    }


    // =========================
    // SHOW QR CODE
    // =========================

    public async Task<IActionResult> ShowQRCode(int id)
    {
        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (teacher == null)
        {
            return NotFound("Teacher profile not found.");
        }

        var session = await _context.ClassSessions
            .Include(cs => cs.Subject)
            .FirstOrDefaultAsync(cs => cs.Id == id);

        if (session == null)
        {
            return NotFound("Class session not found.");
        }

        // Make sure this session belongs to this teacher
        var ownsSubject = await _context.Subjects
            .AnyAsync(s =>
                s.Id == session.SubjectId &&
                s.TeacherId == teacher.Id);

        if (!ownsSubject)
        {
            return Unauthorized();
        }

        return View(session);
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


    // =========================
    // STUDENT DASHBOARD
    // =========================

    

       
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
        .ToListAsync();

    var totalClasses = attendanceRecords.Count;

    var attendedClasses = attendanceRecords.Count(a =>
        a.Status == AttendanceStatus.Present);

    var percentage = totalClasses == 0
        ? 0
        : Math.Round(
            (double)attendedClasses / totalClasses * 100,
            1);

    var subjectSummaries = attendanceRecords
        .Where(a => a.ClassSession != null &&
                    a.ClassSession.Subject != null)
        .GroupBy(a => new
        {
            a.ClassSession!.Subject!.Id,
            a.ClassSession.Subject.Name
        })
        .Select(g => new SubjectAttendanceSummary
        {
            SubjectName = g.Key.Name,
            TotalClasses = g.Count(),
            AttendedClasses = g.Count(a =>
                a.Status == AttendanceStatus.Present),
            
        })
        .ToList();

    var model = new StudentDashboardViewModel
    {
        StudentName = student.User?.FullName ?? "Student",
        RollNumber = student.EnrollmentNo,
        Department = student.Department?.Name ?? "N/A",
        OverallAttendancePercentage = percentage,
        TotalAttended = attendedClasses,
        TotalClassesHeld = totalClasses,
        SubjectSummaries = subjectSummaries
    };

    return View(model);
}



    


    // =========================
    // STUDENT ATTENDANCE HISTORY
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


    // =========================
    // SCAN QR PAGE
    // =========================

    [HttpGet]
    public IActionResult Scan()
    {
        return View();
    }


    // =========================
    // MARK ATTENDANCE
    // =========================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAttendance(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            TempData["Error"] = "Invalid QR code.";
            return RedirectToAction(nameof(Scan));
        }

        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student == null)
        {
            return NotFound("Student profile not found.");
        }

        // Find session using QR token
        var session = await _context.ClassSessions
            .Include(cs => cs.Subject)
            .FirstOrDefaultAsync(cs => cs.QrToken == token);

        if (session == null)
        {
            TempData["Error"] = "Invalid QR code.";
            return RedirectToAction(nameof(Scan));
        }

        // Check QR expiry
        if (!session.QrExpiresAt.HasValue ||
            DateTime.Now > session.QrExpiresAt.Value)
        {
            TempData["Error"] = "This QR code has expired.";
            return RedirectToAction(nameof(Scan));
        }

        // Check duplicate attendance
        var alreadyMarked = await _context.AttendanceRecords
            .AnyAsync(a =>
                a.ClassSessionId == session.Id &&
                a.StudentId == student.Id);

        if (alreadyMarked)
        {
            TempData["Error"] =
                "Your attendance has already been marked for this class.";

            return RedirectToAction(nameof(Scan));
        }

        // Create attendance record
        var attendance = new AttendanceRecord
        {
            ClassSessionId = session.Id,
            StudentId = student.Id,
            Status = AttendanceStatus.Present,
            MarkedAt = DateTime.Now
        };

        _context.AttendanceRecords.Add(attendance);

        await _context.SaveChangesAsync();

        TempData["Success"] =
            $"Attendance marked successfully for {session.Subject?.Name}.";

        return RedirectToAction(nameof(Scan));
    }
}



