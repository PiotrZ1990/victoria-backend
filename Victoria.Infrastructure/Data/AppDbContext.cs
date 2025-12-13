using Microsoft.EntityFrameworkCore;
using Victoria.Domain.Entities;
using Victoria.Domain.Entities.Accommodation;
using Victoria.Domain.Entities.Applications;
using Victoria.Domain.Entities.Cases;
using Victoria.Domain.Entities.CMS;
using Victoria.Domain.Entities.CRM;
using Victoria.Domain.Entities.Documents;
using Victoria.Domain.Entities.Education;
using Victoria.Domain.Entities.Files;
using Victoria.Domain.Entities.Payments;
using Victoria.Domain.Entities.Visa;

namespace Victoria.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        // Konstruktor używany w runtime przez DI
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Konstruktor bezparametrowy do migracji
        public AppDbContext()
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<CaseFile> CaseFiles { get; set; }
        public DbSet<StudyApplication> StudyApplications { get; set; }
        public DbSet<VisaApplication> VisaApplications { get; set; }
        public DbSet<AccommodationRequest> AccommodationRequests { get; set; }
        public DbSet<AccommodationBooking> AccommodationBookings { get; set; }
        public DbSet<FileResource> FileResources { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
        public DbSet<VisaDocument> VisaDocuments { get; set; }
        public DbSet<ApplicationDocumentChecklist> ApplicationDocumentChecklists { get; set; }
        public DbSet<VisaDocumentChecklist> VisaDocumentChecklists { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoicePayment> InvoicePayments { get; set; }
        public DbSet<LanguageCourse> LanguageCourses { get; set; }
        public DbSet<CourseGroup> CourseGroups { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamSession> ExamSessions { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<PageSection> PageSections { get; set; }
        public DbSet<NewsPost> NewsPosts { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<Expert> Experts { get; set; }
        public DbSet<Setting> Settings { get; set; }





        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Jeśli nie ma ustawionych opcji (np. w migracji), ustaw domyślny connection string
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=VictoriaDb;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // LEAD
            // =========================
            modelBuilder.Entity<Lead>()
                .Property(x => x.FullName)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Lead>()
                .Property(x => x.Email)
                .HasMaxLength(200);

            modelBuilder.Entity<Lead>()
                .HasIndex(x => x.Email);

            // =========================
            // CASE FILE
            // =========================
            modelBuilder.Entity<CaseFile>()
                .HasOne(x => x.Lead)
                .WithMany()
                .HasForeignKey(x => x.LeadId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CaseFile>()
                .Property(x => x.CaseNumber)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<CaseFile>()
                .Property(x => x.Stage)
                .HasConversion<int>(); // ENUM

            modelBuilder.Entity<CaseFile>()
                .HasIndex(x => x.CaseNumber)
                .IsUnique();

            // =========================
            // STUDY APPLICATION
            // =========================
            modelBuilder.Entity<StudyApplication>()
                .HasOne(x => x.CaseFile)
                .WithMany()
                .HasForeignKey(x => x.CaseFileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudyApplication>()
                .Property(x => x.Status)
                .HasConversion<int>(); // ENUM

            // =========================
            // VISA
            // =========================
            modelBuilder.Entity<VisaApplication>()
                .HasOne(x => x.CaseFile)
                .WithMany()
                .HasForeignKey(x => x.CaseFileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VisaApplication>()
                .Property(x => x.Status)
                .HasConversion<int>(); // ENUM

            modelBuilder.Entity<VisaApplication>()
                .Property(x => x.VisaType)
                .HasConversion<int>(); // ENUM

            modelBuilder.Entity<VisaApplication>()
                .HasIndex(x => x.Status);

            // =========================
            // ACCOMMODATION
            // =========================
            modelBuilder.Entity<AccommodationRequest>()
                .HasOne(x => x.CaseFile)
                .WithMany()
                .HasForeignKey(x => x.CaseFileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AccommodationRequest>()
                .Property(x => x.Status)
                .HasConversion<int>(); // ENUM

            modelBuilder.Entity<AccommodationRequest>()
                .Property(x => x.BudgetMin)
                .HasPrecision(18, 2);

            modelBuilder.Entity<AccommodationRequest>()
                .Property(x => x.BudgetMax)
                .HasPrecision(18, 2);

            modelBuilder.Entity<AccommodationRequest>()
                .HasIndex(x => x.Status);

            modelBuilder.Entity<AccommodationBooking>()
                .HasOne(x => x.AccommodationRequest)
                .WithOne()
                .HasForeignKey<AccommodationBooking>(x => x.AccommodationRequestId);

            modelBuilder.Entity<AccommodationBooking>()
                .Property(x => x.MonthlyPrice)
                .HasPrecision(18, 2);

            // =========================
            // DOCUMENTS
            // =========================
            modelBuilder.Entity<Document>()
                .HasOne(x => x.FileResource)
                .WithMany()
                .HasForeignKey(x => x.FileResourceId);

            modelBuilder.Entity<ApplicationDocument>()
                .HasOne(x => x.Document)
                .WithMany()
                .HasForeignKey(x => x.DocumentId);

            modelBuilder.Entity<VisaDocument>()
                .HasOne(x => x.Document)
                .WithMany()
                .HasForeignKey(x => x.DocumentId);

            // =========================
            // PAYMENTS & INVOICES
            // =========================
            modelBuilder.Entity<Payment>()
                .HasOne(x => x.CaseFile)
                .WithMany()
                .HasForeignKey(x => x.CaseFileId);

            modelBuilder.Entity<Payment>()
                .Property(x => x.Status)
                .HasConversion<int>(); // ENUM

            modelBuilder.Entity<Payment>()
                .Property(x => x.PaymentMethod)
                .HasConversion<int>(); // ENUM

            modelBuilder.Entity<Payment>()
                .Property(x => x.ServiceType)
                .HasConversion<int>(); // ENUM

            modelBuilder.Entity<Payment>()
                .Property(x => x.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .HasIndex(x => x.PaymentDate);

            modelBuilder.Entity<Invoice>()
                .HasOne(x => x.CaseFile)
                .WithMany()
                .HasForeignKey(x => x.CaseFileId);

            modelBuilder.Entity<Invoice>()
                .Property(x => x.Status)
                .HasConversion<int>(); // ENUM

            modelBuilder.Entity<Invoice>()
                .Property(x => x.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .HasIndex(x => x.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<InvoicePayment>()
                .HasOne(x => x.Invoice)
                .WithMany()
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InvoicePayment>()
                .HasOne(x => x.Payment)
                .WithMany()
                .HasForeignKey(x => x.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // EDUCATION
            // =========================
            modelBuilder.Entity<LanguageCourse>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CourseGroup>()
                .HasOne(x => x.LanguageCourse)
                .WithMany()
                .HasForeignKey(x => x.LanguageCourseId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(x => x.CourseGroup)
                .WithMany()
                .HasForeignKey(x => x.CourseGroupId);

            modelBuilder.Entity<Enrollment>()
                .HasIndex(x => new { x.StudentId, x.CourseGroupId })
                .IsUnique();

            modelBuilder.Entity<Exam>()
                .Property(x => x.ExamType)
                .HasConversion<int>(); // ENUM

            modelBuilder.Entity<ExamSession>()
                .HasOne(x => x.Exam)
                .WithMany()
                .HasForeignKey(x => x.ExamId);

            modelBuilder.Entity<ExamResult>()
                .HasOne(x => x.ExamSession)
                .WithMany()
                .HasForeignKey(x => x.ExamSessionId);

            modelBuilder.Entity<ExamResult>()
                .HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId);

            // =========================
            // CMS
            // =========================
            modelBuilder.Entity<Page>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            modelBuilder.Entity<PageSection>()
                .HasOne(x => x.Page)
                .WithMany()
                .HasForeignKey(x => x.PageId);

            modelBuilder.Entity<Setting>()
                .HasIndex(x => x.Key)
                .IsUnique();
        }


    }

}

