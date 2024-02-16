using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Assignment2.Entities
{
    public enum EnrollmentConfirmationStatus
    {
        ConfirmationMessageNotSent = 0,
        ConfirmationMessageSent = 1,
        EnrollmentConfirmed = 2,
        EnrollmentDeclined = 3
    }

    public class Student
    {
        public int StudentId { get; set; }


        [Required(ErrorMessage = "Who should we invite?")]

        public string? StudentName { get; set; }

        [Required(ErrorMessage = "How should we invite?")]
        [EmailAddress(ErrorMessage = "Must be valid email")]

        public string? StudentEmail { get; set; }

        public EnrollmentConfirmationStatus Status { get; set; } = EnrollmentConfirmationStatus.ConfirmationMessageNotSent;

        public int? CourseId { get; set; }

        public Course? Course { get; set; }
    }
}
