using Microsoft.EntityFrameworkCore;
namespace TestTask.Service;

public interface IEmployeeRepository
{
    public Task<IAsyncEnumerable<Employee>> GetDistinctEmployeesAsync();
    public Task<IAsyncEnumerable<Employee>> GetMaleEmployeesAsync(IEnumerable<Employee> employees);
}

public class EmployeeRepository(IDbContextFactory<AppDbContext> contextFactory) : IEmployeeRepository
{
    public async Task<IAsyncEnumerable<Employee>> GetDistinctEmployeesAsync()
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync();

        return dbContext.Employees
            .DistinctBy(e => new
            {
                e.Name, e.Lastname, e.Surname, e.Birthdate
            })
            .OrderBy(e => e.Lastname)
            .ThenBy(e => e.Name)
            .ThenBy(e => e.Surname)
            .AsAsyncEnumerable();
    }

    public async Task<IAsyncEnumerable<Employee>> GetMaleEmployeesAsync(IEnumerable<Employee> employees)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync();
        return dbContext.Employees.Where(e => e.Gender == Gender.Female && e.Lastname!.StartsWith("F")).AsAsyncEnumerable();
    }
}