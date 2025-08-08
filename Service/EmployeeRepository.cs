using Microsoft.EntityFrameworkCore;
namespace TestTask.Service;

public class EmployeeRepository(IDbContextFactory<AppDbContext> contextFactory) : IEmployeeRepository
{
    public async IAsyncEnumerable<Employee> GetDistinctEmployeesAsync()
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync();

        var sql = """
                  SELECT DISTINCT ON ("Lastname", "Name", "Surname", "Birthdate")
                    "Id", "Birthdate", "Gender", "Lastname", "Name", "Surname"
                  FROM "Employees"
                  ORDER BY "Lastname", "Name", "Surname", "Birthdate", "Id";
                  """;

        var query = dbContext.Employees
            .FromSqlRaw(sql)
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var employee in query)
        {
            yield return employee;
        }
    }

    public async IAsyncEnumerable<Employee> GetMaleEmployeesAsync()
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync();

        var employees = dbContext.Employees
            .AsNoTracking()
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