namespace PFP.Application.Abstractions.Services
{
    // Reversible protection for secrets that must be stored (e.g. SMTP password) and later
    // read back in plaintext to actually use - unlike IPasswordHasher, which is one-way.
    public interface ISecretProtector
    {
        string Protect(string plaintext);

        string Unprotect(string protectedValue);
    }
}
