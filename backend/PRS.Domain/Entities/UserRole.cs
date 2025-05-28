namespace PRS.Domain.Entities;

public class UserRole(string key, string name, string description)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Key { get; set; } = key;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
}