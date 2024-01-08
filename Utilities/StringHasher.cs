using System;
using System.Security.Cryptography;
using System.Text;

namespace ChessApp.Utilities
{
    public class StringHasher
    {
        public static string Hash(string input)
        {
            SHA512 hashAlgorithm = SHA512.Create();
            byte[] byteValue = Encoding.UTF8.GetBytes(input);
            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }
    }
}
