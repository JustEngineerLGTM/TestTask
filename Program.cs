// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TestTask.Model;
using TestTask.Service;

namespace TestTask;

public static class Program
{
    
    private const string? ConnectionString = "";

    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.Configure<GeneratorSettings>(
            builder.Configuration.GetSection("GeneratorSettings"));
        
        builder.Services.AddTransient<RandomGeneratorFactory>();
        builder.Services.AddTransient<CustomGeneratorFactory>();
        builder.Services.AddTransient<IEmpGeneratorFactory>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<GeneratorSettings>>().Value;

            return settings.GeneratorType switch
            {
                GeneratorType.Normal => sp.GetRequiredService<RandomGeneratorFactory>(),
                GeneratorType.Custom => new CustomGeneratorFactory(settings.Gender, settings.FirstChar),
                _ => throw new ArgumentOutOfRangeException()
            };
        });

        builder.Services.AddTransient<EmpGenerate>();
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
            /*Id = dbContext.Employees.Max(e => e.Id),*/
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
        var contextFactory = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();

        await using var dbContext = await contextFactory.CreateDbContextAsync();

        foreach (var employee in dbContext.Employees
                     .DistinctBy(e => new
                     {
                         e.Name, e.Lastname, e.Surname
                     })
                     .OrderBy(e => e.Lastname)
                     .ThenBy(e => e.Name)
                     .ThenBy(e => e.Surname))
        {
            Console.WriteLine(
                $"{employee.Name}  {employee.Lastname} {employee.Surname} {employee.Birthdate} {employee.Gender} {employee.GetAgeYears()} ");
        }
    }

    private static Task Task5(IServiceProvider hostServices)
    {
        var empGenerator = hostServices.GetRequiredService<RandomGeneratorFactory>().CreateGenerator();
        empGenerator.Generate();
        return Task.CompletedTask;
    }

    private static Task Task4(IServiceProvider hostServices)
    {
        throw new NotImplementedException();
    }
}