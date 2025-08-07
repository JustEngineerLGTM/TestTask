namespace TestTask.Service;

public class RandomGeneratorFactory : IEmpGeneratorFactory
{
    public IEmpGenerator CreateGenerator() =>
        new RandomEmpGenerator();
}