using Microsoft.EntityFrameworkCore;
namespace TestTask.Service;

public class EmpGenerators
{
    /// <summary>
    /// Функция генерирует случайного сотрудника, ФИО, возвраст и пол распределены равномерно,
    /// т.к это свойство функции Random.
    /// Фамилии так же расппределены равномерно, потому что в списке их 26, равное количеству букв в англ.алфавите.
    /// </summary>
    /// <returns>Обьект Employee с заполненными полями</returns>
    public Employee Generate()
    {
        var rnd = Random.Shared;
        var employee = new Employee
        {
            Name = FullNames.FirstNames[rnd.Next(0, FullNames.FirstNames.Length)],
            Lastname = FullNames.LastNames[rnd.Next(0, FullNames.LastNames.Length)],
            Surname = FullNames.SurNames[rnd.Next(0, FullNames.SurNames.Length)],
            Birthdate = DateTime.Now.AddYears(-rnd.Next(18, 100)).AddDays(-rnd.Next(365)).ToUniversalTime(),
            Gender = (Gender)rnd.Next(1, 2)
        };
        return employee;
    }

    /// <summary>
    /// Функция генерирует сотрудника, с выбранным полом и начальной буквой.
    /// </summary>
    /// <returns>Обьект Employee с заполненными полями выбранного типа</returns>
    public Employee GenerateCustom(Gender gender, char firstChar)
    {
        Random rnd = Random.Shared;
        var employee = new Employee
        {
            Name = FullNames.FirstNames[rnd.Next(0, FullNames.FirstNames.Length)],
            Lastname = FullNames.LastNames.First(name => name[0] == firstChar),
            Surname = FullNames.SurNames[rnd.Next(0, FullNames.SurNames.Length)],
            Birthdate = DateTime.Now.AddYears(-rnd.Next(18, 100)).AddDays(-rnd.Next(365)).ToUniversalTime(),
            Gender = gender,
        };
        return employee;
    }
}