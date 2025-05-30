namespace PRS.Domain.Entities;

public class UserRole
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public UserRole(string key, string name, string description)
    {
        Key = key;
        Name = name;
        Description = description;
    }

    private UserRole() { }
}