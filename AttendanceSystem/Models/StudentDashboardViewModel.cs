namespace AttendanceSystem.Models
{
    public class StudentDashboardViewModel
    {
        public string StudentName { get; set; }
        public string RollNumber { get; set; }
        public string Department { get; set; }
        public double OverallAttendancePercentage { get; set; }
        public int TotalClassesHeld { get; set; }
        public int TotalAttended { get; set; }
        public List<SubjectAttendanceSummary> SubjectSummaries { get; set; } = new List<SubjectAttendanceSummary>();
    }

    public class SubjectAttendanceSummary
    {
        public string SubjectName { get; set; }
        public int TotalClasses { get; set; }
        public int AttendedClasses { get; set; }
        public double Percentage => TotalClasses > 0 ? (double)AttendedClasses / TotalClasses * 100 : 0;
    }
}