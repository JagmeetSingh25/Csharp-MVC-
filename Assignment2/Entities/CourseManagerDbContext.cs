using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Assignment2.Entities
{
    public class CourseManagerDbContext : DbContext
    {
        public CourseManagerDbContext(DbContextOptions<CourseManagerDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().Property(Student => Student.Status).HasConversion<string>().HasMaxLength(64);
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    CourseId = 1,
                    Name = "Programming Mobile Application",
                    Instructor = "Jagmeet",
                    StartDate = new DateTime(2023, 12, 30),
                    Roomnumber = "2B14"
                }
                );

            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    StudentId = 1,
                    StudentName = "Alice",
                    StudentEmail = "conestogac@conestogac.on.ca",
                    CourseId = 1,
                });
        }
    }
}
