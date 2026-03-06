using AuthService.Domain.Enums;

namespace AuthService.Application.Services;

public interface IAuthService
{
    Task RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        UserRole role,
        CancellationToken cancellationToken = default);

    Task<string?> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}