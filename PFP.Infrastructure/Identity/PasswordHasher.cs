using Microsoft.AspNetCore.Identity;
using PFP.Application.Abstractions.Services;

namespace PFP.Infrastructure.Identity;

internal sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        return _hasher.HashPassword(
            new object(), password);
    }

    public bool Verify(
        string password,
        string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        PasswordVerificationResult result = _hasher.VerifyHashedPassword(
            new object(),
            passwordHash,
            password);

        return result is PasswordVerificationResult.Success or
               PasswordVerificationResult.SuccessRehashNeeded;
    }
}
