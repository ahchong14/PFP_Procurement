using Microsoft.AspNetCore.DataProtection;
using PFP.Application.Abstractions.Services;

namespace PFP.Infrastructure.Security;

// Data Protection's default key ring is persisted per-machine (registry on Windows outside
// a container). That's fine for this project's single-server deployment model, but would
// need an explicit shared key-storage location (e.g. PersistKeysToFileSystem on a shared
// path) before this could run across multiple instances.
internal sealed class DataProtectionSecretProtector : ISecretProtector
{
    private readonly IDataProtector _protector;

    public DataProtectionSecretProtector(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector("PFP.Infrastructure.EmailSettings.Password");
    }

    public string Protect(string plaintext) => _protector.Protect(plaintext);

    public string Unprotect(string protectedValue) => _protector.Unprotect(protectedValue);
}
