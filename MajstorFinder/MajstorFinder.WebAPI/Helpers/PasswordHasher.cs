using System.Security.Cryptography;

namespace MajstorFinder.WebAPI.Helpers
{
    public static class PasswordHasher
    {
        public static void Create(string password, out byte[] hash, out byte[] salt, out int iterations)
        {
            iterations = 100_000;
            salt = RandomNumberGenerator.GetBytes(16);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            hash = pbkdf2.GetBytes(32);
        }

        public static bool Verify(string password, byte[] hash, byte[] salt, int iterations)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var computed = pbkdf2.GetBytes(32);
            return CryptographicOperations.FixedTimeEquals(computed, hash);
        }
    }
}
