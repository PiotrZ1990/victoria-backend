namespace Victoria.Domain.Entities.Education
{
    public class CourseGroup
    {
        public int Id { get; set; }

        public int LanguageCourseId { get; set; }
        public LanguageCourse LanguageCourse { get; set; }

        public string GroupName { get; set; }       // np. EN-APR-2025
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int MaxStudents { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
