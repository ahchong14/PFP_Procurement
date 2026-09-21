namespace PFP.Application.Abstractions.Services;

public interface ITokenService
{
    string GenerateToken(int? userId, int? supplierId, string email, string? role);
}
