using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Models;

public class MyUserContext : IdentityDbContext<MyUser, IdentityRole<int>, int>
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Resume> Resumes { get; set; }
    public DbSet<WorkExperience> WorkExperiences { get; set; } = null!;
    public DbSet<Education> Educations { get; set; } = null!;
    public DbSet<Vacancy> Vacancies { get; set; } = null!;
    public DbSet<JobApplication> JobApplications { get; set; } = null!;
    public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
    public MyUserContext(DbContextOptions<MyUserContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Resume>()
            .Property(r => r.ExpectedSalary)
            .HasPrecision(18, 2);

        builder.Entity<Resume>()
            .HasOne(r => r.Applicant)
            .WithMany()
            .HasForeignKey(r => r.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Resume>()
            .HasOne(r => r.Category)
            .WithMany()
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<WorkExperience>()
            .HasOne(w => w.Resume)
            .WithMany(r => r.WorkExperiences)
            .HasForeignKey(w => w.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Education>()
            .HasOne(e => e.Resume)
            .WithMany(r => r.Educations)
            .HasForeignKey(e => e.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Vacancy>()
            .Property(v => v.Salary)
            .HasPrecision(18, 2);

        builder.Entity<Vacancy>()
            .HasOne(v => v.Employer)
            .WithMany()
            .HasForeignKey(v => v.EmployerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Vacancy>()
            .HasOne(v => v.Category)
            .WithMany()
            .HasForeignKey(v => v.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<JobApplication>()
            .HasOne(a => a.Vacancy)
            .WithMany()
            .HasForeignKey(a => a.VacancyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<JobApplication>()
            .HasOne(a => a.Resume)
            .WithMany()
            .HasForeignKey(a => a.ResumeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<JobApplication>()
            .HasOne(a => a.InitiatedByUser)
            .WithMany()
            .HasForeignKey(a => a.InitiatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<JobApplication>()
            .HasIndex(a => new
            {
                a.VacancyId,
                a.ResumeId
            })
            .IsUnique();
        builder.Entity<ChatMessage>()
            .HasOne(m => m.JobApplication)
            .WithMany()
            .HasForeignKey(m => m.JobApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ChatMessage>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ChatMessage>()
            .HasIndex(m => new
            {
                m.JobApplicationId,
                m.SentAt,
                m.Id
            });
        base.OnModelCreating(builder);
    }
}