// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            ["1"] => Go1(host.Services),
            ["2", .. var rest] => Go2(rest),
            _ => throw new Exception("Неизвестные аргументы")
        };

        await task;
    }
    
    private static async Task Go1(IServiceProvider provider)
    {
        var contextFactory = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        await using var dbContext = await contextFactory.CreateDbContextAsync();

        await dbContext.Database.MigrateAsync();
    }

    private static Task Go2(string[] rest)
    {
        throw new NotImplementedException();
    }
}
