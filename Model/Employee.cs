using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
namespace TestTask;

public class Employee
{
    
    public Guid Id { get; init; }
    // Разделил ФИО на составляющие, т.к. это соответсвутет 1 нормальной форме БД.
    public required string Name { get; init; }
    public required string? Lastname { get; init; }
    public string? Surname { get; init; }
    public required LocalDate Birthdate { get; init; }
    public required Gender Gender { get; init; }

    /// <summary>
    /// Функция вычисляет количество ПОЛНЫХ лет
    /// </summary>
    /// <returns>Количество полных лет</returns>
    public Period GetAge()
    {
        return SystemClock.Instance.GetCurrentInstant().InUtc().Date - Birthdate;
    }

    public async Task InsertInDb(DbContext dbContext)
    {
        dbContext.Add(this);
        await dbContext.SaveChangesAsync();
    }
}