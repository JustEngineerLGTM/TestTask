// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TestTask.Service;

namespace TestTask;

public static class Program
{

    private const string? ConnectionString = "";

    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddDbContextFactory<AppDbContext>(s => s.UseNpgsql(ConnectionString));

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

    private static async Task Task1(IServiceProvider provider)
    {
        var contextFactory = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();

        await using var dbContext = await contextFactory.CreateDbContextAsync();

        await dbContext.Database.MigrateAsync();
    }


    private static async Task Task2(string[] rest, IServiceProvider provider)
    {
        var contextFactory = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();

        await using var dbContext = await contextFactory.CreateDbContextAsync();

        var splitName = rest[0].Split(" ");
        var employee = new Employee
        {
            Name = splitName[0],
            Lastname = splitName[1],
            Surname = splitName[2],
            Birthdate = Convert.ToDateTime(rest[1]),
            Gender = Enum.Parse<Gender>(rest[2])
        };
        await employee.InsertInDb(dbContext);
    }

    private static async Task Task3(IServiceProvider provider)
    {
        var employeeRepository = provider.GetRequiredService<IEmployeeRepository>();
        var request = await employeeRepository.GetDistinctEmployeesAsync();

        await foreach (var employee in request)
        {
            Console.WriteLine($"{employee.Name}  {employee.Lastname} {employee.Surname}" +
                              $" {employee.Birthdate}" +
                              $" {employee.Gender}" +
                              $" {employee.GetAge()}");
        }
    }

    private static async Task Task4(IServiceProvider provider)
    {
        var contextFactory = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();

        await using var dbContext = await contextFactory.CreateDbContextAsync();

        var generators = new EmpGenerators();
        var employees = Enumerable.Range(0, 1000000).Select(_ => generators.Generate());
        var employeesCustom = Enumerable.Range(0, 100).Select(_ => generators.GenerateCustom(Gender.Male, 'F'));
        await dbContext.InsertEmployee(employees.Concat(employeesCustom));
    }

    private static async Task Task5(IServiceProvider provider)
    {
        Stopwatch stopWatch = new Stopwatch();
        var employeeRepository = provider.GetRequiredService<IEmployeeRepository>();
        var request = await employeeRepository.GetDistinctEmployeesAsync();
        stopWatch.Start();
        await foreach (var employee in request)
        {
            Console.WriteLine($"{employee.Name}  {employee.Lastname} {employee.Surname}" +
                              $" {employee.Birthdate}" +
                              $" {employee.Gender}" +
                              $" {employee.GetAge()}");
        }
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Console.WriteLine("RunTime " + elapsedTime);
    }
}