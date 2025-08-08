namespace TestTask.Service;

public interface IEmployeeRepository
{
    IAsyncEnumerable<Employee> GetDistinctEmployeesAsync();
    IAsyncEnumerable<Employee> GetMaleEmployeesAsync();

    Task InsertEmployee(IEnumerable<Employee> employee);
}