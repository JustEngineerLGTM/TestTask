using Microsoft.EntityFrameworkCore;
namespace TestTask;

public class AppDbContext : DbContext
{
    protected AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    
    public async Task InsertEmployee(IEnumerable<Employee> employee)
    {
        await AddRangeAsync(employee);
        await SaveChangesAsync();
    }
}