namespace TestTask.Service;

public class CustomEmpGenerator(Gender gender, char firstChar) : IEmpGenerator
{
    public Employee Generate()
    {
        Random rnd = new Random();
        var employee = new Employee
        {
            Name = FullNames.FirstNames[rnd.Next(0, FullNames.FirstNames.Length)],
            Lastname = FullNames.LastNames.Where(name => name[0] == firstChar).ToString(),
            Surname = FullNames.LastNames[rnd.Next(0, FullNames.LastNames.Length)],
            Birthdate = DateTime.Now.AddYears(-rnd.Next(18, 100)).AddYears(-rnd.Next(365)),
            Gender = gender,
        };
        return employee;
    }
}