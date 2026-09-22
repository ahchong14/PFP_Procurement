using PFP.Domain.Interface;

namespace PFP.Domain.Entities
{
    // Single-row configuration, Id is always 1 - seeded once by ApplicationDbContextSeed.cs.
    public class EmailSettings : IEntity, IExposableEntity
    {
        public int Id { get; set; }

        public required string Host { get; set; }

        public int Port { get; set; }

        public required string Username { get; set; }

        // Protected via ISecretProtector before being stored - never a plaintext password.
        public required string EncryptedPassword { get; set; }

        public required string FromEmail { get; set; }

        public required string FromName { get; set; }
    }
}
