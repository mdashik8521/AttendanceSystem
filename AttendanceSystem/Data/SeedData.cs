using AttendanceSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Data
{
    public static class SeedData
    {
        public static readonly string[] Roles =
        {
            "Admin",
            "Teacher",
            "Student"
        };

        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();
            var roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();

            // ==========================================
            // DATABASE MIGRATION
            // ==========================================

            await context.Database.MigrateAsync();

            // ==========================================
            // 1. CREATE ROLES
            // ==========================================

            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role)
                    );
                }
            }

            // ==========================================
            // 2. CREATE ADMIN
            // ==========================================

            await CreateUser(
                userManager,
                "admin@attendance.com",
                "Admin@123",
                "System Admin",
                "Admin"
            );

            // ==========================================
            // 3. CREATE 10 DEPARTMENTS
            // ==========================================

            var departmentNames = new[]
            {
                "B.Tech",
                "BCA",
                "BBA",
                "MCA",
                "Artificial Intelligence & Machine Learning",
                "Artificial Intelligence & Data Science",
                "Computer Engineering",
                "Information Technology",
                "Electronics & Communication",
                "Mechanical Engineering"
            };

            foreach (var departmentName in departmentNames)
            {
                var exists = await context.Departments
                    .AnyAsync(d => d.Name == departmentName);

                if (!exists)
                {
                    context.Departments.Add(
                        new Department
                        {
                            Name = departmentName
                        }
                    );
                }
            }

            await context.SaveChangesAsync();

            var departments = await context.Departments
                .OrderBy(d => d.Id)
                .ToListAsync();

            if (departments.Count == 0)
            {
                throw new Exception("No departments found.");
            }

            // ==========================================
            // 4. ACTUAL STUDENT DATA
            // ==========================================

            var studentData = new[]
            {
                new
                {
                    GrNo = 121591,
                    Enrollment = "92301703042",
                    Name = "ONABUREKHALEN OJIE",
                    Email = "onaburekhalenojie.121591@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 123708,
                    Enrollment = "92301703237",
                    Name = "DUENG ABEN AYUL ANGUI",
                    Email = "duengabenayulangui.123708@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 132257,
                    Enrollment = "92400103011",
                    Name = "PRANAV DHARMESH MEHTA",
                    Email = "pranav.mehta132257@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 131900,
                    Enrollment = "92400103031",
                    Name = "PRIYANSHU KUMAR",
                    Email = "priyanshu.kumar131900@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 131993,
                    Enrollment = "92400103035",
                    Name = "KRISHNA DHANKANI",
                    Email = "krishnadhankani.131993@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 132801,
                    Enrollment = "92400103051",
                    Name = "PRINCE KUMAR",
                    Email = "prince.kumar132801@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 131766,
                    Enrollment = "92400103214",
                    Name = "JEETRAJSINH PANKAJSINH GOHIL",
                    Email = "jeetrajsinh.gohil131766@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 125276,
                    Enrollment = "92400103216",
                    Name = "MAHMAD SAJID SIKANDARBHAI POPATPAUTRA",
                    Email = "mahmadsajid.popatpautra125276@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 128721,
                    Enrollment = "92400103223",
                    Name = "RAHUL KUMAR",
                    Email = "rahulkumar.128721@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 131685,
                    Enrollment = "92400103238",
                    Name = "ANKUSH KUMAR",
                    Email = "ankush.kumar131685@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 126090,
                    Enrollment = "92400103292",
                    Name = "SHREYANSH RAJABHAI BHALIYA",
                    Email = "shreyansh.bhaliya126090@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 133391,
                    Enrollment = "92400103300",
                    Name = "BHAVATI JAVIA",
                    Email = "bhavatijavia.133391@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 126308,
                    Enrollment = "92400103307",
                    Name = "PARTH JAYESHBHAI TANK",
                    Email = "parth.tank126308@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 131215,
                    Enrollment = "92400103308",
                    Name = "HEET VINODBHAI JAVIYA",
                    Email = "heet.javiya131215@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 126364,
                    Enrollment = "92400103312",
                    Name = "MATUL KUMAR",
                    Email = "matul.kumar126364@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 126432,
                    Enrollment = "92400103316",
                    Name = "BHAVY MAYUR PUJARA",
                    Email = "bhavy.pujara126432@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 126547,
                    Enrollment = "92400103319",
                    Name = "PURVI DILIPBHAI VAGHAMSHI",
                    Email = "vaghamshi.dilipbhai126547@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 126630,
                    Enrollment = "92400103322",
                    Name = "OMDEEPSINH SIDDHRAJSINH JADEJA",
                    Email = "omdeepsinh.jadeja126630@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 128544,
                    Enrollment = "92400103350",
                    Name = "KUNAL RITESHBHAI HIRANI",
                    Email = "kunal.hirani128544@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 129440,
                    Enrollment = "92400103375",
                    Name = "HINESH KANANI",
                    Email = "hinesh.kanani129440@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 130173,
                    Enrollment = "92400103412",
                    Name = "DEVANSHIBA NARENDRASINH ZALA",
                    Email = "devanshiba.zala130173@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 130212,
                    Enrollment = "92400103415",
                    Name = "PALAK THAWANI",
                    Email = "palakthawani.130212@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 128638,
                    Enrollment = "92400103443",
                    Name = "SHAILESH KUMAR YADAV",
                    Email = "shailesh.128638@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 130720,
                    Enrollment = "92400103461",
                    Name = "RUSHABH JITENBHAI TOLIA",
                    Email = "rushabh.tolia130720@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 131183,
                    Enrollment = "92400103480",
                    Name = "MANCOBA MDUMISENI DUBE",
                    Email = "mancobamdumisenidube.131183@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 131224,
                    Enrollment = "92400103483",
                    Name = "ISHA VINAYBHAI THAKER",
                    Email = "isha.thaker131224@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 130833,
                    Enrollment = "92400103485",
                    Name = "MD ASHIK",
                    Email = "mdashik.akhatar130833@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 130837,
                    Enrollment = "92400103486",
                    Name = "AATIF EQUBAL",
                    Email = "aatif.equbal130837@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 133227,
                    Enrollment = "92400103529",
                    Name = "JAY RUSHIBHAI VISANA",
                    Email = "jay.visana133227@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 133234,
                    Enrollment = "92400103530",
                    Name = "RONAK DEVENDRABHAI RAM",
                    Email = "ronak.ram133234@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 133344,
                    Enrollment = "92400103539",
                    Name = "DIVYARAJSINH MAHENDRASINH ZALA",
                    Email = "divyarajsinh.zala133344@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 133461,
                    Enrollment = "92400103541",
                    Name = "PARTH JAYENDRABHAI HIRANI",
                    Email = "parth.hirani133461@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 133511,
                    Enrollment = "92400103546",
                    Name = "AYUSH NARANBHAI VADARIYA",
                    Email = "ayush.vadariya133511@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 133582,
                    Enrollment = "92400103550",
                    Name = "JAINISH KAKKAD",
                    Email = "jainishkakkad.133582@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 144061,
                    Enrollment = "92510103006",
                    Name = "KIRTAN HARSHADRAI CHAUHAN",
                    Email = "kirtan.chauhan144061@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 138534,
                    Enrollment = "92510103009",
                    Name = "MAYUR MANSUKHBHAI CHAVDA",
                    Email = "mayur.chavda138534@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 137041,
                    Enrollment = "92510103018",
                    Name = "TEJ ASHVINBHAI BHALODI",
                    Email = "tej.bhalodi137041@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 142077,
                    Enrollment = "92510103044",
                    Name = "POOJAN RAJENDRABHAI MAKADIA",
                    Email = "poojan.makadia142077@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 142516,
                    Enrollment = "92510103053",
                    Name = "DHRUV BHARATBHAI CHAVADA",
                    Email = "dhruv.chavada142516@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 142533,
                    Enrollment = "92510103055",
                    Name = "INDRAPRIT MANOJBHAI JADAV",
                    Email = "indraprit.jadav142533@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 142654,
                    Enrollment = "92510103056",
                    Name = "JAY BHAVESH DETROJA",
                    Email = "jay.detroja142654@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 142520,
                    Enrollment = "92510103060",
                    Name = "PREET KAILASHBHAI HANSALIYA",
                    Email = "preet.hansaliya142520@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 142647,
                    Enrollment = "92510103061",
                    Name = "MEET KAILASHBHAI AMBALIYA",
                    Email = "meet.ambaliya142647@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 142987,
                    Enrollment = "92510103064",
                    Name = "PRIT SANJAYBHAI PANSURIYA",
                    Email = "prit.pansuriya142987@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 143040,
                    Enrollment = "92510103065",
                    Name = "MOHMADFAISAL GULABRASUL BADI",
                    Email = "mohmadfaisal.badi143040@marwadiuniversity.ac.in"
                },
                new
                {
                    GrNo = 143223,
                    Enrollment = "92510103074",
                    Name = "BERA OM",
                    Email = "beraom.143223@marwadiuniversity.ac.in"
                }
            };

            // ==========================================
            // 5. CREATE / UPDATE STUDENTS
            // ==========================================

            foreach (var data in studentData)
            {
                var studentEmail = data.Email;

                var studentUser =
                    await userManager.FindByEmailAsync(studentEmail);

                if (studentUser == null)
                {
                    studentUser = new ApplicationUser
                    {
                        UserName = studentEmail,
                        Email = studentEmail,
                        FullName = data.Name,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(
                        studentUser,
                        "Student@123"
                    );

                    if (!result.Succeeded)
                    {
                        throw new Exception(
                            $"Failed to create {studentEmail}: " +
                            string.Join(
                                ", ",
                                result.Errors.Select(
                                    e => e.Description)
                            )
                        );
                    }
                }
                else
                {
                    studentUser.FullName = data.Name;
                    studentUser.Email = data.Email;
                    studentUser.UserName = data.Email;

                    var result =
                        await userManager.UpdateAsync(studentUser);

                    if (!result.Succeeded)
                    {
                        throw new Exception(
                            $"Failed to update {studentEmail}: " +
                            string.Join(
                                ", ",
                                result.Errors.Select(
                                    e => e.Description)
                            )
                        );
                    }
                }

                if (!await userManager.IsInRoleAsync(
                    studentUser,
                    "Student"))
                {
                    var roleResult =
                        await userManager.AddToRoleAsync(
                            studentUser,
                            "Student");

                    if (!roleResult.Succeeded)
                    {
                        throw new Exception(
                            $"Failed to add Student role to {studentEmail}"
                        );
                    }
                }

                // Find existing student profile by UserId
                var studentProfile =
                    await context.Students
                        .FirstOrDefaultAsync(
                            s => s.UserId == studentUser.Id);

                // Use first department for this class
                var department = departments[0];

                // Last 4 digits of enrollment number
                var enrollmentNo =
                    data.Enrollment[^4..];

                if (studentProfile == null)
                {
                    context.Students.Add(
                        new Student
                        {
                            Id = data.GrNo,
                            UserId = studentUser.Id,
                            EnrollmentNo = enrollmentNo,
                            Semester = 5,
                            DepartmentId = department.Id
                        }
                    );
                }
                else
                {
                    studentProfile.Id = data.GrNo;
                    studentProfile.EnrollmentNo = enrollmentNo;
                    studentProfile.Semester = 5;
                    studentProfile.DepartmentId = department.Id;
                }
            }

            await context.SaveChangesAsync();

            // ==========================================
            // 6. REMOVE OLD STUDENT PROFILES
            // ==========================================

            var validEmails = studentData
                .Select(s => s.Email)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var allStudentProfiles =
                await context.Students
                    .Include(s => s.User)
                    .ToListAsync();

            var oldStudents = allStudentProfiles
                .Where(s =>
                    s.User == null ||
                    string.IsNullOrEmpty(s.User.Email) ||
                    !validEmails.Contains(s.User.Email))
                .ToList();

            if (oldStudents.Any())
            {
                context.Students.RemoveRange(oldStudents);
                await context.SaveChangesAsync();

                foreach (var oldStudent in oldStudents)
                {
                    if (oldStudent.User != null)
                    {
                        await userManager.DeleteAsync(
                            oldStudent.User
                        );
                    }
                }
            }


// ==========================================
// DEMO STUDENT ACCOUNT
// ==========================================

var demoStudent = await userManager.FindByEmailAsync(
    "student@attendance.com");

if (demoStudent == null)
{
    demoStudent = new ApplicationUser
    {
        UserName = "student@attendance.com",
        Email = "student@attendance.com",
        FullName = "Demo Student",
        EmailConfirmed = true
    };

    var result = await userManager.CreateAsync(
        demoStudent,
        "Student@123"
    );

    if (!result.Succeeded)
    {
        throw new Exception(
            "Failed to create demo student: " +
            string.Join(
                ", ",
                result.Errors.Select(e => e.Description)
            )
        );
    }
}

if (!await userManager.IsInRoleAsync(
    demoStudent,
    "Student"))
{
    await userManager.AddToRoleAsync(
        demoStudent,
        "Student");
}

var demoStudentProfile =
    await context.Students
        .FirstOrDefaultAsync(
            s => s.UserId == demoStudent.Id);

if (demoStudentProfile == null)
{
    context.Students.Add(
        new Student
        {
            UserId = demoStudent.Id,
            EnrollmentNo = "DEMO001",
            Semester = 5,
            DepartmentId = departments[0].Id
        }
    );

    await context.SaveChangesAsync();
}




            // ==========================================
            // 7. TEACHER NAMES - 20 TEACHERS
            // ==========================================

            var teacherNames = new[]
            {
                "Dr. Jaydeep Ratanpara",
                "Prof. Neha Shah",
                "Dr. Amit Mehta",
                "Prof. Priya Desai",
                "Dr. Kiran Patel",
                "Prof. Rakesh Shah",
                "Dr. Pooja Mehta",
                "Prof. Nitin Desai",
                "Dr. Seema Patel",
                "Prof. Rahul Shah",
                "Dr. Anjali Mehta",
                "Prof. Manish Desai",
                "Dr. Kavita Patel",
                "Prof. Suresh Shah",
                "Dr. Rina Mehta",
                "Prof. Deepak Desai",
                "Dr. Swati Patel",
                "Prof. Vijay Shah",
                "Dr. Mehul Mehta",
                "Prof. Sneha Desai"
            };

            // ==========================================
            // 8. CREATE / UPDATE 20 TEACHERS
            // ==========================================

            for (int i = 1; i <= 20; i++)
            {
                var teacherEmail = i == 1
                    ? "teacher@attendance.com"
                    : $"teacher{i}@attendance.com";

                var teacherUser =
                    await userManager.FindByEmailAsync(
                        teacherEmail);

                if (teacherUser == null)
                {
                    teacherUser = new ApplicationUser
                    {
                        UserName = teacherEmail,
                        Email = teacherEmail,
                        FullName = teacherNames[i - 1],
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(
                        teacherUser,
                        "Teacher@123"
                    );

                    if (!result.Succeeded)
                    {
                        throw new Exception(
                            $"Failed to create {teacherEmail}: " +
                            string.Join(
                                ", ",
                                result.Errors.Select(
                                    e => e.Description)
                            )
                        );
                    }
                }
                else
                {
                    teacherUser.FullName =
                        teacherNames[i - 1];

                    var result =
                        await userManager.UpdateAsync(
                            teacherUser);

                    if (!result.Succeeded)
                    {
                        throw new Exception(
                            $"Failed to update {teacherEmail}: " +
                            string.Join(
                                ", ",
                                result.Errors.Select(
                                    e => e.Description)
                            )
                        );
                    }
                }

                if (!await userManager.IsInRoleAsync(
                    teacherUser,
                    "Teacher"))
                {
                    await userManager.AddToRoleAsync(
                        teacherUser,
                        "Teacher");
                }

                var teacherProfile =
                    await context.Teachers
                        .FirstOrDefaultAsync(
                            t => t.UserId == teacherUser.Id);

                var department =
                    departments[(i - 1) % departments.Count];

                if (teacherProfile == null)
                {
                    context.Teachers.Add(
                        new Teacher
                        {
                            UserId = teacherUser.Id,
                            DepartmentId = department.Id
                        }
                    );
                }
                else
                {
                    teacherProfile.DepartmentId =
                        department.Id;
                }
            }

            await context.SaveChangesAsync();

            // ==========================================
            // 9. CREATE / UPDATE SUBJECTS
            // ==========================================

            var subjectData = new[]
            {
    new
    {
        Name = ".NET",
        Code = "DOTNET",
        Semester = 5,
        TeacherIndex = 1
    },
    new
    {
        Name = "Data Science Essentials",
        Code = "DSE",
        Semester = 5,
        TeacherIndex = 2
    },
    new
    {
        Name = "Design and Analysis of Algorithms",
        Code = "DAA",
        Semester = 5,
        TeacherIndex = 3
    },
    new
    {
        Name = "Artificial Intelligence",
        Code = "AI",
        Semester = 5,
        TeacherIndex = 4
    },
    new
    {
        Name = "Financial Mathematics",
        Code = "FM",
        Semester = 5,
        TeacherIndex = 5
    },
    new
    {
        Name = "Quantitative and Logical Aptitude",
        Code = "QLA",
        Semester = 5,
        TeacherIndex = 6
    },
    new
    {
        Name = "Open Source Technologies",
        Code = "OST",
        Semester = 5,
        TeacherIndex = 7
    },
    new
    {
        Name = "Database Management System",
        Code = "DBMS",
        Semester = 5,
        TeacherIndex = 8
    }
};

            var teachers = await context.Teachers
                .OrderBy(t => t.Id)
                .ToListAsync();

            foreach (var data in subjectData)
            {
                var teacher = teachers[data.TeacherIndex - 1];

                var subject = await context.Subjects
                    .FirstOrDefaultAsync(
                        s => s.Code == data.Code
                    );

                if (subject == null)
                {
                    context.Subjects.Add(
                        new Subject
                        {
                            Name = data.Name,
                            Code = data.Code,
                            Semester = data.Semester,
                            TeacherId = teacher.Id
                        }
                    );
                }
                else
                {
                    subject.Name = data.Name;
                    subject.Semester = data.Semester;
                    subject.TeacherId = teacher.Id;
                }
            }

            await context.SaveChangesAsync();

            // ==========================================
            // 10. CREATE 6 CLASS SESSIONS
            // ==========================================

            var subjects = await context.Subjects
                .Where(s =>
                    s.Code == "CS301" ||
                    s.Code == "CS302" ||
                    s.Code == "CS303" ||
                    s.Code == "CS304" ||
                    s.Code == "CS305" ||
                    s.Code == "CS306")
                .OrderBy(s => s.Code)
                .ToListAsync();

            var sessionData = new[]
            {
                new
                {
                    SubjectCode = "CS301",
                    Date = new DateTime(2026, 9, 1),
                    StartTime = new TimeSpan(9, 0, 0)
                },
                new
                {
                    SubjectCode = "CS302",
                    Date = new DateTime(2026, 9, 2),
                    StartTime = new TimeSpan(10, 0, 0)
                },
                new
                {
                    SubjectCode = "CS303",
                    Date = new DateTime(2026, 9, 3),
                    StartTime = new TimeSpan(11, 0, 0)
                },
                new
                {
                    SubjectCode = "CS304",
                    Date = new DateTime(2026, 9, 4),
                    StartTime = new TimeSpan(9, 0, 0)
                },
                new
                {
                    SubjectCode = "CS305",
                    Date = new DateTime(2026, 9, 5),
                    StartTime = new TimeSpan(10, 0, 0)
                },
                new
                {
                    SubjectCode = "CS306",
                    Date = new DateTime(2026, 9, 6),
                    StartTime = new TimeSpan(11, 0, 0)
                }
            };

            foreach (var data in sessionData)
            {
                var subject =
                    subjects.First(
                        s => s.Code == data.SubjectCode);

                var sessionExists =
                    await context.ClassSessions.AnyAsync(
                        cs =>
                            cs.SubjectId == subject.Id &&
                            cs.Date == data.Date &&
                            cs.StartTime == data.StartTime);

                if (!sessionExists)
                {
                    context.ClassSessions.Add(
                        new ClassSession
                        {
                            SubjectId = subject.Id,
                            Date = data.Date,
                            StartTime = data.StartTime,
                            QrToken = null,
                            QrExpiresAt = null
                        }
                    );
                }
            }

            await context.SaveChangesAsync();

            // ==========================================
            // SEED COMPLETE
            // ==========================================
        }

        // ==========================================
        // HELPER METHOD
        // ==========================================

        private static async Task CreateUser(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string fullName,
            string role)
        {
            var user =
                await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true
                };

                var result =
                    await userManager.CreateAsync(
                        user,
                        password);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Failed to create {email}: " +
                        string.Join(
                            ", ",
                            result.Errors.Select(
                                e => e.Description)
                        )
                    );
                }
            }
            else
            {
                user.FullName = fullName;

                var result =
                    await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Failed to update {email}: " +
                        string.Join(
                            ", ",
                            result.Errors.Select(
                                e => e.Description)
                        )
                    );
                }
            }

            if (!await userManager.IsInRoleAsync(
                user,
                role))
            {
                await userManager.AddToRoleAsync(
                    user,
                    role);
            }
        }
    }
}

