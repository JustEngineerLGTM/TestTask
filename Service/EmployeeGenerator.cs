namespace TestTask.Service;

public class EmpGenerate(IEmpGeneratorFactory factory)
{

    public async Task CreateEmpRecord(AppDbContext context, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var generator = factory.CreateGenerator();
            context.Employees.Add(generator.Generate());
        }
        await context.SaveChangesAsync();
    }

}