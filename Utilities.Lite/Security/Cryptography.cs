using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Utilities.Security
{
    //taking the implementation based-on https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp with some different tuning parameter
    /// <summary>
    /// Simple wrapper for cryptography methods
    /// </summary>
    public static class Cryptography
    {
        #region Simple Encryption - Decryption Method

        /// <summary>
        /// Encrypt given plain text into hash string with specific salt
        /// </summary>
        /// <param name="plainText">Plain text to encrypt</param>
        /// <param name="salt">Salt using to calculate the hash</param>
        /// <param name="blockSize">Entropy size</param>
        /// <param name="iterations">Encryption iterations</param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string salt, int blockSize = 128, int iterations = 2000)
        {
            var saltStringBytes = GenerateRandomEntropy(blockSize);
            var ivStringBytes = GenerateRandomEntropy(blockSize);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(salt, saltStringBytes, iterations))
            {
                var keyBytes = password.GetBytes(blockSize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = blockSize;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decrypt given hash string into plain text with specific salt
        /// </summary>
        /// <param name="hash">Hash string to decrypt</param>
        /// <param name="salt">Salt using to calculate the hash</param>
        /// <param name="blockSize">Entropy size</param>
        /// <param name="iterations">Decryption iterations</param>
        /// <returns></returns>
        public static string Decrypt(string hash, string salt, int blockSize = 128, int iterations = 2000)
        {
            var blockSizeByte = 128 / 8;
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(hash);
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(blockSizeByte).ToArray();
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(blockSizeByte).Take(blockSizeByte).ToArray();
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((blockSizeByte) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((blockSizeByte) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(salt, saltStringBytes, iterations))
            {
                var keyBytes = password.GetBytes(blockSizeByte);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = blockSize;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        #endregion Simple Encryption - Decryption Method

        /// <summary>
        /// Randomly generate salt byte array
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateSalt()
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var salt = new byte[32];
                randomNumberGenerator.GetBytes(salt);
                return salt;
            }
        }

        private static byte[] GenerateRandomEntropy(int blockSize)
        {
            var randomBytes = new byte[blockSize / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
        /// <summary>
        /// Provide statistically random string generate with customizable length and combination.
        /// </summary>
        /// <param name="length">Length of string.</param>
        /// <param name="combination">Combination of string.</param>
        /// <returns></returns>
        public static string SecureRandomString(int length, string combination = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890")
        {
            var result = new StringBuilder(length);
            using (var rng = new RNGCryptoServiceProvider())
            {
                int count = (int)Math.Ceiling(Math.Log(combination.Length, 2) / 8.0);
                int offset = BitConverter.IsLittleEndian ? 0 : sizeof(uint) - count;
                int max = (int)(Math.Pow(2, count * 8) / combination.Length) * combination.Length;
                byte[] uintBuffer = new byte[sizeof(uint)];
                while (result.Length < length)
                {
                    rng.GetBytes(uintBuffer, offset, count);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    if (num < max)
                    {
                        result.Append(combination[(int)(num % combination.Length)]);
                    }

                }
            }
            return result.ToString();
        }
        internal static byte[] SecureGetBytes(this string key)
        {
            Encoding enc = Encoding.UTF8;
            using SHA256 sha2 = new SHA256CryptoServiceProvider();
            byte[] rawKey = enc.GetBytes(key);
            byte[] hashKey = sha2.ComputeHash(rawKey);
            return hashKey;
        }
    }
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
        public static string Encrypt(string text, string salt)
        {
            var combined = Combine(text, salt);
            var data = Encoding.UTF8.GetBytes(combined);
            using var shaM = new SHA256Managed();
            var res = shaM.ComputeHash(data);
            return Convert.ToBase64String(res);
        }
        /// <summary>
        /// Verify if given text is the same with the hash by encrypt it and compare it againts the hash.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="hash"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static bool Verify(string text, string hash, string salt)
        {
            var challengeHash = Encrypt(text, salt);
            return challengeHash.Equals(hash);
        }
        private static string Combine(string text, string salt)
        {
            var saltFlip = salt.Reverse();
            var combinedText = new StringBuilder();
            var rules = new[] { 2, 5, 7 };
            for (var i = 0; i < text.Length; i++)
            {
                combinedText.Append(text[i]);
                if (i != 0)
                    foreach (var rule in rules)
                        if (i % rule == 0)
                            combinedText.Append(salt[i]);
            }
            var leftOver = new string(saltFlip.Skip(combinedText.Length).ToArray());
            combinedText.Append(leftOver);
            return combinedText.ToString();
        }
    }
}