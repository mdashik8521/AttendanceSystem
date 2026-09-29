// ============================================================
// Attendance Management System - Models + DbContext
// Split each class into its own file under /Models and /Data
// when you add it to the project (shown as sections below).
// Requires: Microsoft.AspNetCore.Identity.EntityFrameworkCore,
//           Microsoft.EntityFrameworkCore.SqlServer
// ============================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Models
{
    // ---------- Models/ApplicationUser.cs ----------
    // Roles (create in Program.cs seed): "Admin", "Teacher", "Student"
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;
    }

    // ---------- Models/Department.cs ----------
    public class Department
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    }

    // ---------- Models/Student.cs ----------
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required, StringLength(30)]
        public string EnrollmentNo { get; set; } = string.Empty;

        [Range(1, 8)]
        public int Semester { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }

    // ---------- Models/Teacher.cs ----------
    public class Teacher
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }

    // ---------- Models/Subject.cs ----------
    public class Subject
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Range(1, 8)]
        public int Semester { get; set; }

        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public ICollection<ClassSession> Sessions { get; set; } = new List<ClassSession>();
    }

    // ---------- Models/ClassSession.cs ----------
    // One lecture. The QR token lets students mark themselves present
    // until ExpiresAt (your "innovation" feature).
    public class ClassSession
    {
        public int Id { get; set; }

        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        public TimeSpan StartTime { get; set; }

        public string? QrToken { get; set; }
        public DateTime? QrExpiresAt { get; set; }

        public ICollection<AttendanceRecord> Records { get; set; } = new List<AttendanceRecord>();
    }

    // ---------- Models/AttendanceStatus.cs ----------
    public enum AttendanceStatus
    {
        Present = 1,
        Absent = 2,
        Late = 3
    }

    // ---------- Models/AttendanceRecord.cs ----------
    public class AttendanceRecord
    {
        public int Id { get; set; }

        public int ClassSessionId { get; set; }
        public ClassSession? ClassSession { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public AttendanceStatus Status { get; set; }

        public DateTime MarkedAt { get; set; } = DateTime.Now;
    }
}

