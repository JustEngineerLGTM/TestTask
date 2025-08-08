// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using TestTask.Service;

namespace TestTask;

public static class Program
{

    private const string? ConnectionString = "Server=127.0.0.1;Port=5432;Database=postgres;User Id=postgres;Password=pass;";

    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddDbContextFactory<AppDbContext>(s => s.UseNpgsql(ConnectionString, b => b.UseNodaTime()));
        builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        //builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);

        var host = builder.Build();
        var task = args switch
        {
            [] => throw new Exception("Арументов не передано"),
            ["1"] => Task1(host.Services),
            ["2", .. var rest] => Task2(rest, host.Services),
            ["3"] => Task3(host.Services),
            ["4"] => Task4(host.Services),
            ["5"] => Task5(host.Services),
            _ => throw new Exception("Неизвестные аргументы")
        };

        await task;
    }

    /// <summary>
    /// Создание таблицы с полями справочника сотрудников, представляющими "Фамилию Имя Отчество", "дату рождения", "пол".
    /// </summary>
    private static async Task Task1(IServiceProvider provider)
    {
        var contextFactory = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();

        await using var dbContext = await contextFactory.CreateDbContextAsync();
        // Создаем миграцию, в бд создается таблица Employee
        await dbContext.Database.MigrateAsync();
    }

    /// <summary>
    /// Создание записи справочника сотрудников.
    /// Для работы с данными создать класс и создавать объекты.
    /// При вводе создавать новый объект класса, с введенными пользователем данными.
    /// При генерации строчек в базу создавать объект
    /// и его отправлять в базу/формировать строчку для отправки нескольких строк в БД.
    /// У объекта должны быть методы, которые
    /// - отправляют объект в БД,
    /// - рассчитывают возраст (полных лет).
    /// </summary>
    private static async Task Task2(string[] rest, IServiceProvider provider)
    {
        var contextFactory = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();

        await using var dbContext = await contextFactory.CreateDbContextAsync();

        var splitName = rest[0].Split(" ");
        var employee = new Employee
        {
            Lastname = splitName[0],
            Name = splitName[1],
            Surname = splitName[2],
            Birthdate = LocalDate.FromDateOnly(DateOnly.Parse(rest[1])),
            Gender = Enum.Parse<Gender>(rest[2])
        };
        await employee.InsertInDb(dbContext);
    }

    /// <summary>
    /// Вывод всех строк справочника сотрудников, с уникальным значением ФИО+дата, отсортированным по ФИО.
    /// Вывести ФИО, Дату рождения, пол, кол-во полных лет.
    /// </summary>
    private static async Task Task3(IServiceProvider provider)
    {
        var employeeRepository = provider.GetRequiredService<IEmployeeRepository>();
        var request = employeeRepository.GetDistinctEmployeesAsync();

        await foreach (var employee in request)
        {
            Console.WriteLine($"{employee.Lastname} {employee.Name} {employee.Surname} " +
                              $"\t{employee.Birthdate} " +
                              $"\t{employee.Gender} " +
                              $"\t{employee.GetAge().Years}");
        }
    }

    /// <summary>
    /// Заполнение автоматически 1000000 строк справочника сотрудников.
    /// Распределение пола в них должно быть относительно равномерным, начальной буквы ФИО также.
    /// Добавить заполнение автоматически 100 строк в которых пол мужской и Фамилия начинается с "F".
    /// У класса необходимо создать метод, который пакетно отправляет данные в БД, принимая массив объектов.
    /// </summary>
    /// <param name="provider"></param>
    private static async Task Task4(IServiceProvider provider)
    {
        var employeeRepository = provider.GetRequiredService<IEmployeeRepository>();
        var generators = new EmpGenerators();
        // Генерируем 1.000.000 сотрудников рандомно
        var employees = Enumerable.Range(0, 1000000).Select(_ => generators.Generate());
        // Генерируем 100 сотрудников мужского пола на букву F
        var employeesCustom = Enumerable.Range(0, 100).Select(_ => generators.GenerateCustom(Gender.Male, 'F'));
        await employeeRepository.InsertEmployee(employees.Concat(employeesCustom));
    }

    /// <summary>
    /// Результат выборки из таблицы по критерию: пол мужской, Фамилия начинается с "F".
    /// Сделать замер времени выполнения.
    /// </summary>
    private static async Task Task5(IServiceProvider provider)
    {
        Stopwatch stopWatch = new Stopwatch();
        var employeeRepository = provider.GetRequiredService<IEmployeeRepository>();
        var request = employeeRepository.GetMaleEmployeesAsync();
        stopWatch.Start();

        await foreach (var employee in request)
        {
            Console.WriteLine($"{employee.Lastname} {employee.Name} {employee.Surname} " +
                              $"\t{employee.Birthdate} " +
                              $"\t{employee.Gender} ");
        }

        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        // Форматируем формат времени, для красивого вывода

        Console.WriteLine("RunTime " + ts);
    }
}