using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace ContosoUniversity.Data;

public class SchoolContext : DbContext
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
    public DbSet<CourseAssignment> CourseAssignments { get; set; }

    public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) 
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>().ToTable("Course");
        modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
        modelBuilder.Entity<Student>().ToTable("Student");
        modelBuilder.Entity<Department>().ToTable("Department");
        modelBuilder.Entity<Instructor>().ToTable("Instructor");
        modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");
        modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment");

        modelBuilder.Entity<CourseAssignment>()
            .HasKey(c => new { CourseId = c.CourseId, InstructorId = c.InstructorId });
    }

    private IDbContextTransaction _currentTransaction;

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction is not null)
        {
            return;
        }

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            await (_currentTransaction?.CommitAsync() ?? Task.CompletedTask);
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
