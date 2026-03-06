using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;

namespace AuthService.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
    }

    public async Task RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var emailExists = await _userRepository.EmailExistsAsync(normalizedEmail, cancellationToken);
        if (emailExists)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        var passwordHash = _passwordHasher.Hash(password);

        var user = new User(
            firstName,
            lastName,
            normalizedEmail,
            passwordHash,
            role);

        await _userRepository.AddAsync(user, cancellationToken);
    }

    public async Task<string?> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
        {
            return null;
        }

        var isPasswordValid = _passwordHasher.Verify(password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return null;
        }

        if (!user.IsActive)
        {
            return null;
        }

        var accessToken = _tokenProvider.GenerateAccessToken(user);
        return accessToken;
    }
}