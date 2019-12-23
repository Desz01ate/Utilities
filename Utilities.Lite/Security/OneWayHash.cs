using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Utilities.Shared;

namespace Utilities.Security
{
#if NETSTANDARD2_1
    /// <summary>
    /// Simple wrapper for one-way hashing specifically for password hashing.
    /// </summary>
    public static class OneWayHash
    {
        /// <summary>
        /// Encrypt text with specified salt, the encrypted text is irreversible but still can be verify using Verify(text,hash,salt) method.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"/>
        public static string Encrypt(string text, byte[] salt, DeriveBytes? deriveBytes = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (salt.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(salt));
            }
            using var encryptor = deriveBytes ?? new Rfc2898DeriveBytes(text, salt, 100000, HashAlgorithmName.SHA512);
            var res = encryptor.GetBytes(24);
            var sb = new StringBuilder();
            foreach (var _byte in res)
            {
                sb.Append(_byte.ToString("x2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// Encrypt text WITHOUT specified salt and let the internal worker generate salt instead, the encrypted text is irreversible but still can be verify using Verify(text,hash,salt) method.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string Encrypt(string text, out byte[] salt)
        {
            salt = Cryptography.GenerateSalt(256);
            return Encrypt(text, salt);
        }
        /// <summary>
        /// Verify if given text is the same with the hash by encrypt it and compare it againts the hash.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="hash"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"/>
        public static bool Verify(string text, string hash, byte[] salt)
        {
            var challengeHash = Encrypt(text, salt);
            return challengeHash.Equals(hash);
        }
        private static readonly int[] rules = new int[] { 2, 5, 7 };
    }
#endif
}
