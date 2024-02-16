using Assignment2.Entities;
using Microsoft.Extensions.Logging;

namespace Assignment2.Services
{
    public interface ICourseManagerService
    {

        public List<Course> GetAllCourses();
        public Course? GetCourseById(int id);


        public int AddCourse(Course study);


        public void UpdateCourse(Course study);

        public Student? GetStudentById(int courseId, int studentId);

        public void UpdateConfirmationStatus(int courseId, int studentId, EnrollmentConfirmationStatus status);

        public Course? AddStudentToCourseById(int courseId, Student student);

        public void SendEnrollmentEmailByEventId(int courseID, string scheme, string host);


    }
}
