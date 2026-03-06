using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces;

public interface ITokenProvider
{
    string GenerateAccessToken(User user);
}