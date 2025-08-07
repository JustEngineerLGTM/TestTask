using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace TestTask;

public class Employee
{
    
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string? Lastname { get; init; }
    public string? Surname { get; init; }
    public required DateTime Birthdate { get; init; }
    public required Gender Gender { get; init; }

    public int GetAge()
    {
        return DateTime.Now.Year - Birthdate.Year;
    }

    public async Task InsertInDb(DbContext dbContext)
    {
        dbContext.Add(this);
        await dbContext.SaveChangesAsync();
    }
}