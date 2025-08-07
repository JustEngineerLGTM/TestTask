namespace TestTask.Service;

public class CustomGeneratorFactory(Gender gender, char firstChar) : IEmpGeneratorFactory
{
    public IEmpGenerator CreateGenerator() =>
        new CustomEmpGenerator(gender, firstChar);
}