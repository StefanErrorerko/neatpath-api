using NeatPath.Interfaces;
using System.Security.Cryptography;

namespace NeatPath.Helpers
{
    public class TokenGenerator : ITokenGenerator
    {
        public string GenerateToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

    }
}
