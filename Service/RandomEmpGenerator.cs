namespace TestTask.Service;

public class RandomEmpGenerator : IEmpGenerator
{
    public Employee Generate()
    {
        var rnd = new Random();
        var employee = new Employee
        {
            Name = FullNames.FirstNames[rnd.Next(0, FullNames.FirstNames.Length)],
            Lastname = FullNames.LastNames[rnd.Next(0, FullNames.LastNames.Length)],
            Surname = FullNames.LastNames[rnd.Next(0, FullNames.LastNames.Length)],
            Birthdate = DateTime.Now.AddYears(-rnd.Next(18, 100)).AddYears(-rnd.Next(365)),
            Gender = (Gender)rnd.Next(1, 2)
        };
        return employee;
    }
}