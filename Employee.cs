namespace TestTask;

public class Employee
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Lastname { get;set; }
    public string? Surname { get;set; }
    public required DateTime Birthdate { get;set; }
    public required Gender Gender { get; set; }
}