using Microsoft.EntityFrameworkCore;
namespace TestTask.Service;

public class EmployeeRepository(IDbContextFactory<AppDbContext> contextFactory) : IEmployeeRepository
{
    public async IAsyncEnumerable<Employee> GetDistinctEmployeesAsync()
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync();

        var employees = dbContext.Employees
            .OrderBy(e => e.Lastname ?? string.Empty)
            .ThenBy(e => e.Name)
            .ThenBy(e => e.Surname ?? string.Empty)
            .GroupBy(e => new
            {
                e.Name, e.Lastname, e.Surname, e.Birthdate
            })
            .Select(g => g.First())
            .AsAsyncEnumerable();

        await foreach (var employee in employees)
        {
            yield return employee;
        }
    }

    public async IAsyncEnumerable<Employee> GetMaleEmployeesAsync()
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync();

        var employees = dbContext.Employees
            .Where(e => e.Gender == Gender.Male && e.Lastname!.StartsWith("F"))
            .AsAsyncEnumerable();

        await foreach (var employee in employees)
        {
            yield return employee;
        }
    }

    public async Task InsertEmployee(IEnumerable<Employee> employee)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync();
        await dbContext.AddRangeAsync(employee);
        await dbContext.SaveChangesAsync();
    }
}