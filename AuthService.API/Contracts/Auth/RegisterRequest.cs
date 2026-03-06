using AuthService.Domain.Enums;

namespace AuthService.API.Contracts.Auth;

public sealed record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    UserRole Role);