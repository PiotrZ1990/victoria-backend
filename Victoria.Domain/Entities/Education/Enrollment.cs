using Victoria.Domain.Entities;

namespace Victoria.Domain.Entities.Education
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseGroupId { get; set; }
        public CourseGroup CourseGroup { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}
