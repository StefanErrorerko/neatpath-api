using NeatPath.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace NeatPath.Helpers
{
    public class UrlService : IUrlService
    {
        private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public string HashUrl(string longUrl)
        {
            // Here os algorithm of hashing url. Link: https://blog.algomaster.io/p/design-a-url-shortener
            // 1. Generate MD5 hash
            byte[] inputBytes = Encoding.UTF8.GetBytes(longUrl);
            byte[] hashBytes = MD5.HashData(inputBytes);

            // 2. Take first 6 bytes
            byte[] shortened = new byte[6];
            Array.Copy(hashBytes, shortened, 6);

            // 3. Convert to decimal (as a long)
            long decimalValue = BitConverter.ToInt64(shortened.Concat(new byte[2]).ToArray(), 0);
            decimalValue = Math.Abs(decimalValue);

            // 4. Convert to Base62
            return ToBase62(decimalValue);
        }

        public bool VerifyUrl(string longUrl, string hash)
        {
            return hash == HashUrl(longUrl);
        }

        private string ToBase62(long value)
        {
            if (value == 0) return Alphabet[0].ToString();

            var sb = new StringBuilder();
            while (value > 0)
            {
                sb.Insert(0, Alphabet[(int)(value % 62)]);
                value /= 62;
            }

            while (sb.Length < 6)
            {
                sb.Insert(0, Alphabet[0]);
            }

            return sb.ToString();
        }
    }
}

