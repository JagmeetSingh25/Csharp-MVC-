using System.ComponentModel.DataAnnotations;

namespace Assignment2.Entities
{
    public class Course
    {

        public int CourseId { get; set; }

        [Required(ErrorMessage = "What's your Course Name?")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Who is the instructor?")]
        public string? Instructor { get; set; }

        [Required(ErrorMessage = "When's your Course start?")]
        public DateTime? StartDate { get; set; }


        [Required(ErrorMessage = "Where's your Course?")]
        [RegularExpression(@"^\d[A-Z]\d{2}$", ErrorMessage = "Must be in a format 1X11")]

        public string? Roomnumber { get; set; }


        public List<Student>? Students { get; set; }
    }
}
