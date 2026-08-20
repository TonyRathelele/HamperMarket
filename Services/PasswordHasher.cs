using System.Security.Cryptography;

namespace HamperMarket.Services
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public static (string Hash, string Salt) Hash(string password)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
            var keyBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA256, KeySize);
            return (Convert.ToBase64String(keyBytes), Convert.ToBase64String(saltBytes));
        }

        public static bool Verify(string password, string hash, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var expected = Convert.FromBase64String(hash);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA256, KeySize);
            return CryptographicOperations.FixedTimeEquals(expected, actual);
        }
    }
}
