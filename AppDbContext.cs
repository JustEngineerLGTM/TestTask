using Microsoft.EntityFrameworkCore;
namespace TestTask;

public class AppDbContext : DbContext
{
    protected AppDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Employee>().HasIndex(employee => new
        {
            employee.Gender, employee.Lastname
        });
    }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }

}