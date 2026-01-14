using Microsoft.EntityFrameworkCore;
using Victoria.Domain.Entities;
using Victoria.Domain.Entities.Accommodation;
using Victoria.Domain.Entities.Applications;
using Victoria.Domain.Entities.Cases;
using Victoria.Domain.Entities.Cms;
using Victoria.Domain.Entities.CMS;
using Victoria.Domain.Entities.CRM;
using Victoria.Domain.Entities.Documents;
using Victoria.Domain.Entities.Education;
using Victoria.Domain.Entities.Files;
using Victoria.Domain.Entities.Payments;
using Victoria.Domain.Entities.Staff;
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
        public DbSet<Domain.Entities.Cms.Teacher> Teachers { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<CaseFile> CaseFiles { get; set; }
        public DbSet<StudyApplication> StudyApplications { get; set; }
        public DbSet<VisaApplication> VisaApplications => Set<VisaApplication>();
        public DbSet<AccommodationRequest> AccommodationRequests { get; set; }
        public DbSet<AccommodationBooking> AccommodationBookings { get; set; }
        public DbSet<FileResource> FileResources => Set<FileResource>();
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
        public DbSet<VisaDocument> VisaDocuments => Set<VisaDocument>();
        public DbSet<ApplicationDocumentChecklist> ApplicationDocumentChecklists { get; set; }
        public DbSet<VisaDocumentChecklist> VisaDocumentChecklists => Set<VisaDocumentChecklist>();
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoicePayment> InvoicePayments { get; set; }
        public DbSet<LanguageCourse> LanguageCourses { get; set; }
        public DbSet<CourseGroup> CourseGroups { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamSession> ExamSessions { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<Victoria.Domain.Entities.Cms.Page> Pages => Set<Victoria.Domain.Entities.Cms.Page>();
        public DbSet<PageSection> PageSections { get; set; }
        public DbSet<Domain.Entities.CMS.NewsPost> NewsPosts => Set<NewsPost>();
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<Expert> Experts { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<CaseApplicationChecklistItem> CaseApplicationChecklistItems => Set<CaseApplicationChecklistItem>();
        public DbSet<CaseVisaChecklistItem> CaseVisaChecklistItems => Set<CaseVisaChecklistItem>();
        public DbSet<AccommodationDocumentChecklist> AccommodationDocumentChecklists => Set<AccommodationDocumentChecklist>();
        public DbSet<CaseAccommodationChecklistItem> CaseAccommodationChecklistItems => Set<CaseAccommodationChecklistItem>();
        public DbSet<Employee> Employees => Set<Employee>();





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

            modelBuilder.Entity<Document>()
                .Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();


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
            
            // (opcjonalnie ale polecam) UpdatedAt
            modelBuilder.Entity<LanguageCourse>()
                .Property(x => x.UpdatedAt)
                .IsRequired(false);

            // (bonus: naprawi warning o ExamResult.Score jeśli nadal jest)
            modelBuilder.Entity<ExamResult>()
                .Property(x => x.Score)
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
            // Case checklist items (per CaseFile)
            // =========================

            modelBuilder.Entity<CaseApplicationChecklistItem>()
                .HasOne(x => x.CaseFile)
                .WithMany()
                .HasForeignKey(x => x.CaseFileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CaseApplicationChecklistItem>()
                .HasOne(x => x.Checklist)
                .WithMany()
                .HasForeignKey(x => x.ChecklistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CaseApplicationChecklistItem>()
                .HasOne(x => x.Document)
                .WithMany()
                .HasForeignKey(x => x.DocumentId)
                .OnDelete(DeleteBehavior.SetNull);

            // 1 pozycja checklisty (szablon) ma wystąpić tylko raz na sprawę
            modelBuilder.Entity<CaseApplicationChecklistItem>()
                .HasIndex(x => new { x.CaseFileId, x.ChecklistId })
                .IsUnique();


            modelBuilder.Entity<CaseVisaChecklistItem>()
                .HasOne(x => x.CaseFile)
                .WithMany()
                .HasForeignKey(x => x.CaseFileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CaseVisaChecklistItem>()
                .HasOne(x => x.Checklist)
                .WithMany()
                .HasForeignKey(x => x.ChecklistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CaseVisaChecklistItem>()
                .HasOne(x => x.Document)
                .WithMany()
                .HasForeignKey(x => x.DocumentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CaseVisaChecklistItem>()
                .HasIndex(x => new { x.CaseFileId, x.ChecklistId })
                .IsUnique();

            modelBuilder.Entity<AccommodationDocumentChecklist>()
                .Property(x => x.DocumentType)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<AccommodationDocumentChecklist>()
                .HasIndex(x => x.DocumentType)
                .IsUnique();

            modelBuilder.Entity<CaseAccommodationChecklistItem>()
                .HasOne(x => x.Checklist)
                .WithMany()
                .HasForeignKey(x => x.ChecklistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CaseAccommodationChecklistItem>()
                .HasIndex(x => new { x.CaseFileId, x.ChecklistId })
                .IsUnique();

            modelBuilder.Entity<Employee>(e =>
            {
                e.Property(x => x.FullName).HasMaxLength(200).IsRequired();
                e.Property(x => x.Email).HasMaxLength(200).IsRequired();
                e.Property(x => x.PhoneNumber).HasMaxLength(50).IsRequired();
                e.Property(x => x.Position).HasMaxLength(100).IsRequired();

                e.HasIndex(x => x.Email).IsUnique();
            });


            // =========================
            // CMS
            // =========================
            modelBuilder.Entity<Page>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            modelBuilder.Entity<PageSection>()
                 .HasOne(x => x.Page)
                 .WithMany(p => p.Sections)
                 .HasForeignKey(x => x.PageId);


            modelBuilder.Entity<Setting>()
                .HasIndex(x => x.Key)
                .IsUnique();

            modelBuilder.Entity<ApplicationDocumentChecklist>().HasData(
                new ApplicationDocumentChecklist { Id = 1, DocumentType = "Passport scan", IsRequired = true },
                new ApplicationDocumentChecklist { Id = 2, DocumentType = "CV", IsRequired = true },
                new ApplicationDocumentChecklist { Id = 3, DocumentType = "Motivation letter", IsRequired = true },
                new ApplicationDocumentChecklist { Id = 4, DocumentType = "School certificates / diplomas", IsRequired = true },
                new ApplicationDocumentChecklist { Id = 5, DocumentType = "English test certificate (optional)", IsRequired = false },
                new ApplicationDocumentChecklist { Id = 6, DocumentType = "Proof of funds (if needed)", IsRequired = false }
            );
            modelBuilder.Entity<VisaDocumentChecklist>().HasData(
                new VisaDocumentChecklist { Id = 1, DocumentType = "Passport", IsRequired = true },
                new VisaDocumentChecklist { Id = 2, DocumentType = "Acceptance letter", IsRequired = true },
                new VisaDocumentChecklist { Id = 3, DocumentType = "Visa application form", IsRequired = true },
                new VisaDocumentChecklist { Id = 4, DocumentType = "Proof of accommodation", IsRequired = false },
                new VisaDocumentChecklist { Id = 5, DocumentType = "Travel insurance", IsRequired = false },
                new VisaDocumentChecklist { Id = 6, DocumentType = "Bank statements / proof of funds", IsRequired = true }
            );
            // =========================
            // SEED: FileResources + Documents (do testów checklist)
            // =========================

            modelBuilder.Entity<FileResource>().HasData(
                new FileResource
                {
                    Id = 1,
                    FileName = "passport-scan-demo.pdf",
                    ContentType = "application/pdf",
                    FileSize = 123456,
                    FilePath = "seed/passport-scan-demo.pdf",
                    UploadedAt = new DateTime(2025, 1, 1)
                },
                new FileResource
                {
                    Id = 2,
                    FileName = "acceptance-letter-demo.pdf",
                    ContentType = "application/pdf",
                    FileSize = 234567,
                    FilePath = "seed/acceptance-letter-demo.pdf",
                    UploadedAt = new DateTime(2025, 1, 1)
                }
            );

            modelBuilder.Entity<Document>().HasData(
                new Document
                {
                    Id = 1,
                    DocumentType = "Passport scan (DEMO)",   
                    Status = Domain.Enums.DocumentStatus.Uploaded,
                    FileResourceId = 1,
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Document
                {
                    Id = 2,
                    DocumentType = "Acceptance letter (DEMO)",
                    Status = Domain.Enums.DocumentStatus.Uploaded,
                    FileResourceId = 2,
                    CreatedAt = new DateTime(2025, 1, 1)
                }
            );


        }


    }

}

