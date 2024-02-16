using Assignment2.Entities;
using Assignment2.Models;
using Assignment2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Assignment2.Controllers
{
    public class CourseController : AbstractBaseController
    {
        private readonly ICourseManagerService _courseManagerService;

        public CourseController(ICourseManagerService courseManagerService)
        {
            _courseManagerService = courseManagerService;
        }

        [HttpGet("/courses")]
        public IActionResult List()
        {
            SetWelcome();

            var coursesViewModel = new CoursesViewModel()
            {
                Courses = _courseManagerService.GetAllCourses(),
            };

            return View(coursesViewModel);
        }

        [HttpGet("/courses/{id:int}")]
        public IActionResult Manage(int id)
        {
            SetWelcome();

            var party = _courseManagerService.GetCourseById(id);

            var manageCourseViewModel = new ManageCourseViewModel()
            {
                Course = party,
                Student = new Entities.Student(),
                CountConfirmationMessageNotSent = party.Students.Count(g => g.Status == EnrollmentConfirmationStatus.ConfirmationMessageNotSent),
                CountConfirmationMessageSent = party.Students.Count(g => g.Status == EnrollmentConfirmationStatus.ConfirmationMessageSent),
                CountEnrollmentConfirmed = party.Students.Count(g => g.Status == EnrollmentConfirmationStatus.EnrollmentConfirmed),
                CountEnrollmentDeclined = party.Students.Count(g => g.Status == EnrollmentConfirmationStatus.EnrollmentDeclined),

            };
            return View(manageCourseViewModel);
        }

        [HttpGet("/courses/{courseId:int}/enroll/{studentId:int}")]

        public IActionResult Enroll(int courseId, int studentId)
        {
            SetWelcome();

            var student = _courseManagerService.GetStudentById(courseId, studentId);

            if (student == null)
            {
                return NotFound();
            }

            var enrollStudentViewModel = new EnrollStudentViewModel()
            {
                Student = student,
            };

            return View(enrollStudentViewModel);
        }


        [HttpPost("/courses/{courseId:int}/enroll/{studentId:int}")]

        public IActionResult Enroll(int courseId, int studentId, EnrollStudentViewModel enrollStudentViewModel)
        {
            SetWelcome();

            var student = _courseManagerService.GetStudentById(courseId, studentId);

            if (student == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var status = enrollStudentViewModel.Response == "Yes"
                    ? EnrollmentConfirmationStatus.EnrollmentConfirmed :
                    EnrollmentConfirmationStatus.EnrollmentDeclined;

                _courseManagerService.UpdateConfirmationStatus(courseId, studentId, status);

                return RedirectToAction("ThankYou", new { response = enrollStudentViewModel.Response });
            }
            else
            {
                enrollStudentViewModel.Student = student;
                return View(enrollStudentViewModel);
            }

        }
        [HttpGet("/courses/add")]

        public IActionResult Add()
        {
            SetWelcome();

            var courseViewModel = new CourseViewModel()
            {
                Course = new Entities.Course()
            };
            return View(courseViewModel);
        }

        [HttpPost("/courses/add")]

        public IActionResult Add(CourseViewModel courseViewModel)
        {
            SetWelcome();
            if (!ModelState.IsValid) return View(courseViewModel);


            _courseManagerService.AddCourse(courseViewModel.Course);

            TempData["notify"] = $"{courseViewModel.Course.Name} added successfully";
            TempData["className"] = "success";

            return RedirectToAction("Manage", new { id = courseViewModel.Course.CourseId });
        }

        [HttpGet("/courses/{id:int}/edit")]

        public IActionResult Edit(int id)
        {
            SetWelcome();

            var courseViewModel = new CourseViewModel()
            {
                Course = _courseManagerService.GetCourseById(id)
            };
            return View(courseViewModel);
        }

        [HttpPost("/courses/{id:int}/edit")]


        public IActionResult Edit(CourseViewModel courseViewModel, int id)
        {
            SetWelcome();



            if (!ModelState.IsValid) return View(courseViewModel.Course);

            _courseManagerService.UpdateCourse(courseViewModel.Course);

            TempData["notify"] = $"{courseViewModel.Course.Name} update successfully";
            TempData["className"] = "info";

            return RedirectToAction("Manage", new { id });
        }

        [HttpGet("/thank-you/{response}")]

        public IActionResult ThankYou(string response)
        {
            SetWelcome();

            return View("ThankYou", response);
        }

        [HttpPost("/courses/{courseId:int}/add-student")]

        public IActionResult AddStudent(int courseId, ManageCourseViewModel manageCourseViewModel)
        {
            SetWelcome();

            Course? study;

            if (ModelState.IsValid)
            {
                study = _courseManagerService.AddStudentToCourseById(courseId, manageCourseViewModel.Student);

                if (study == null) { return NotFound(); }

                TempData["notify"] = $"{manageCourseViewModel.Student.StudentName} added to guest list ";
                TempData["className"] = "success";



                return RedirectToAction("Manage", new { id = courseId });
            }
            else
            {
                study = _courseManagerService.GetCourseById(courseId);
                
                if (study == null) { return NotFound(); }

                manageCourseViewModel.Course = study;
                manageCourseViewModel.CountConfirmationMessageNotSent = study.Students.Count(g => g.Status == EnrollmentConfirmationStatus.ConfirmationMessageNotSent);
                manageCourseViewModel.CountConfirmationMessageSent = study.Students.Count(g => g.Status == EnrollmentConfirmationStatus.ConfirmationMessageSent);
                manageCourseViewModel.CountEnrollmentConfirmed = study.Students.Count(g => g.Status == EnrollmentConfirmationStatus.EnrollmentConfirmed);
                manageCourseViewModel.CountEnrollmentDeclined = study.Students.Count(g => g.Status == EnrollmentConfirmationStatus.EnrollmentDeclined);
                return View("Manage", manageCourseViewModel);
            }
        }

        [HttpPost("courses/{courseId:int}/enroll")]
        public IActionResult SendInvitation(int courseId)
        {
            _courseManagerService.SendEnrollmentEmailByEventId(courseId, Request.Scheme, Request.Host.ToString());
            return RedirectToAction("Manage", new { id = courseId });
        }
    }
}
