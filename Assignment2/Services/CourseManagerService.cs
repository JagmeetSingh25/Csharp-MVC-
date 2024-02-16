using Assignment2.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;

namespace Assignment2.Services
{
    public class CourseManagerService : ICourseManagerService
    {
        private readonly IConfiguration _configuration;
        private readonly CourseManagerDbContext _courseManagerDbContext;

        public CourseManagerService(CourseManagerDbContext courseManagerDbContext, IConfiguration configuration)
        {
            _courseManagerDbContext = courseManagerDbContext;
            _configuration = configuration;

        }


        public List<Course> GetAllCourses()
        {
            return _courseManagerDbContext.Courses.Include(e => e.Students).OrderByDescending(e => e.StartDate).ToList();
        }

        public Course? GetCourseById(int id)
        {
            return _courseManagerDbContext.Courses.Include(e => e.Students).FirstOrDefault(e => e.CourseId == id);
        }

        public int AddCourse(Course study)
        {
            _courseManagerDbContext.Courses.Add(study);
            _courseManagerDbContext.SaveChanges();
            return study.CourseId;
        }

        public void UpdateCourse(Course study)
        {
            _courseManagerDbContext.Courses.Update(study);
            _courseManagerDbContext.SaveChanges();

        }

        public Student? GetStudentById(int courseId, int studentId)
        {
            return _courseManagerDbContext.Students
                .Include(g => g.Course)
                .FirstOrDefault(g => g.CourseId == courseId && g.StudentId == studentId);
        }

        public void UpdateConfirmationStatus(int courseId, int studentId, EnrollmentConfirmationStatus status)
        {
            var student = GetStudentById(courseId, studentId);

            if (student == null)
            {
                return;
            }

            student.Status = status;
            _courseManagerDbContext.SaveChanges();
        }
        public Course? AddStudentToCourseById(int courseId, Student student)
        {
            var study = GetCourseById(courseId);

            if (study == null) { return null; }

            study.Students?.Add(student);
            _courseManagerDbContext.SaveChanges();

            return study;
        }

        public void SendEnrollmentEmailByEventId(int courseID, string scheme, string host)
        {
            var study = GetCourseById(courseID);

            if (study == null) return;

            var students = study.Students.Where(g => g.Status == EnrollmentConfirmationStatus.ConfirmationMessageNotSent).ToList();

            try
            {
                var smtpHost = _configuration["SmtpSettings:Host"];
                var smtpPort = _configuration["SmtpSettings:Port"];
                var fromAddress = _configuration["SmtpSettings:FromAddress"];
                var fromPassword = _configuration["SmtpSettings:FromPassword"];

                using var smtpClient = new SmtpClient(smtpHost);
                smtpClient.Port = string.IsNullOrEmpty(smtpPort) ? 587 : Convert.ToInt32(smtpPort);
                smtpClient.Credentials = new NetworkCredential(fromAddress, fromPassword);
                smtpClient.EnableSsl = true;

                foreach (var student in students)
                {

                    var responseUrl = $"{scheme}://{host}/events/{courseID}/enroll/{student.StudentId}";
                    var mailMessage = new MailMessage()
                    {
                        From = new MailAddress(fromAddress),
                        Subject = $"{{Action Required}} Confirm \"{student?.Course?.Name}\" Enrollment",
                        Body = CreateBoody(student, responseUrl),
                        IsBodyHtml = true
                    };
                    if (student.StudentEmail == null) { return; }
                    mailMessage.To.Add(student.StudentEmail);

                    smtpClient.Send(mailMessage);
                    student.Status = EnrollmentConfirmationStatus.ConfirmationMessageSent;
                }

                _courseManagerDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private string CreateBoody(Student student, string responseUrl)
        {
            return $@"
            <h1>Hello {student.StudentName}.</h1>

             <p> 
                Your request to enroll in the course {student.Course.Name}.
                in room {student.Course.Roomnumber}
                starting {student.Course.StartDate:d}
                 with host {student.Course.Instructor}
             </p>

             <p>
               This is a very important notice , which you should pay attention more to as it will be coming helpful in the future.
                <a href={responseUrl}>confirm your enrollment</a>

                


            <p> Sincerly </p>
            <p> The Event Manager App </p>

            ";
        }
    }
}
