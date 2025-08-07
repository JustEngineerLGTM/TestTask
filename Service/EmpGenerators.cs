using Microsoft.EntityFrameworkCore;
namespace TestTask.Service;

public class EmpGenerators
{
    public Employee Generate()
    {
        var rnd = Random.Shared;
        var employee = new Employee
        {
            Name = FullNames.FirstNames[rnd.Next(0, FullNames.FirstNames.Length)],
            Lastname = FullNames.LastNames[rnd.Next(0, FullNames.LastNames.Length)],
            Surname = FullNames.SurNames[rnd.Next(0, FullNames.SurNames.Length)],
            Birthdate = DateTime.Now.AddYears(-rnd.Next(18, 100)).AddYears(-rnd.Next(365)),
            Gender = (Gender)rnd.Next(1, 2)
        };
        return employee;
    }
    public Employee GenerateCustom(Gender gender, char firstChar)
    {
        Random rnd = Random.Shared;
        var employee = new Employee
        {
            Name = FullNames.FirstNames[rnd.Next(0, FullNames.FirstNames.Length)],
            Lastname = FullNames.LastNames.First(name => name[0] == firstChar),
            Surname = FullNames.SurNames[rnd.Next(0, FullNames.SurNames.Length)],
            Birthdate = DateTime.Now.AddYears(-rnd.Next(18, 100)).AddYears(-rnd.Next(365)),
            Gender = gender,
        };
        return employee;
    }
}