using AuthService.Domain.Enums;
namespace AuthService.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private User()
    {
    }

    public User(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        UserRole role)
    {
        Id = Guid.NewGuid();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
        Role = role;
    }
}