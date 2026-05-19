using Microsoft.EntityFrameworkCore;
using rosa_testovoye.Data.Entities;

namespace rosa_testovoye.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<CertificateRequest> CertificateRequests => Set<CertificateRequest>();

    public DbSet<RequestStatusHistory> RequestStatusHistory => Set<RequestStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.UserName)
                .IsUnique();

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Department)
                .HasMaxLength(200);

            entity.Property(e => e.Role)
                .HasConversion<int>();
        });

        modelBuilder.Entity<CertificateRequest>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Reason)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(r => r.CustomTypeName)
                .HasMaxLength(200);

            entity.Property(r => r.Type)
                .HasConversion<int>();

            entity.Property(r => r.Status)
                .HasConversion<int>();

            entity.HasOne(r => r.Employee)
                .WithMany(e => e.Requests)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => new { r.EmployeeId, r.Type, r.Status });
        });

        modelBuilder.Entity<RequestStatusHistory>(entity =>
        {
            entity.HasKey(h => h.Id);

            entity.Property(h => h.Comment)
                .HasMaxLength(1000);

            entity.Property(h => h.FromStatus)
                .HasConversion<int>();

            entity.Property(h => h.ToStatus)
                .HasConversion<int>();

            entity.HasOne(h => h.Request)
                .WithMany(r => r.StatusHistory)
                .HasForeignKey(h => h.RequestId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(h => h.ChangedByEmployee)
                .WithMany(e => e.StatusChanges)
                .HasForeignKey(h => h.ChangedByEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
