using Assignment2.Entities;
using Microsoft.Extensions.Logging;

namespace Assignment2.Models
{
    public class ManageCourseViewModel
    {
        public Course? Course { get; set; }

        public Student? Student { get; set; }

        public int CountConfirmationMessageNotSent { get; set; }
        public int CountConfirmationMessageSent { get; set; }
        public int CountEnrollmentConfirmed { get; set; }
        public int CountEnrollmentDeclined { get; set; }
    }
}
