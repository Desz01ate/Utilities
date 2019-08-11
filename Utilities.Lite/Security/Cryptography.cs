using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Utilities.Structs;

namespace Utilities
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
        #endregion

        #region Simple Hash/Salt Generator and Verification
        /// <summary>
        /// Generate combination of hash string and salt string from given plain text and salt byte array
        /// </summary>
        /// <param name="plainText">Plain text to encrypt</param>
        /// <param name="salt">Salt using to calculate the hash</param>
        /// <param name="byteSize">Entropy size</param>
        /// <param name="iterations">Encryption iterations</param>
        /// <returns></returns>
        public static HashCombination GenerateHash(string plainText, byte[] salt, int byteSize = 128, int iterations = 2000)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var rfc2898 = new Rfc2898DeriveBytes(plainTextBytes, salt, iterations))
            {
                var hash = rfc2898.GetBytes(byteSize);
                return new HashCombination(Convert.ToBase64String(hash), Convert.ToBase64String(salt));
            }
        }
        /// <summary>
        /// Generate combination of hash string and salt string from given plain text
        /// </summary>
        /// <param name="plainText">Plain text to encrypt</param>
        /// <param name="byteSize">Entropy size</param>
        /// <param name="iterations">Encryption iterations</param>
        /// <returns></returns>
        public static HashCombination GenerateHash(string plainText, int byteSize = 128, int iterations = 2000)
        {
            return GenerateHash(plainText, GenerateSalt(), byteSize, iterations);
        }
        /// <summary>
        /// Generate combination of hash string and salt string from given plain text and salt base64 string
        /// </summary>
        /// <param name="plainText">Plain text to encrypt</param>
        /// <param name="salt">Base64 string using to calculate the hash</param>
        /// <param name="byteSize">Entropy size</param>
        /// <param name="iterations">Encryption iterations</param>
        /// <returns></returns>
        public static HashCombination GenerateHash(string plainText, string salt, int byteSize = 128, int iterations = 2000)
        {
            var saltBytes = Convert.FromBase64String(salt);
            return GenerateHash(plainText, saltBytes, byteSize, iterations);
        }
        /// <summary>
        /// Compare the plain text with given hash and salt
        /// </summary>
        /// <param name="plainText">Plain text to compare</param>
        /// <param name="hash">Hash string to compare</param>
        /// <param name="salt">Salt to compute while comparing</param>
        /// <param name="iterations">Iterations to compute while comparing</param>
        /// <returns></returns>
        public static bool Verify(string plainText, string hash, string salt, int iterations = 2000)
        {
            var inputHash = GenerateHash(plainText, salt, iterations: iterations);
            return inputHash.Hash == hash;
        }
        #endregion
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
#if NET452
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    result.Append(combination[(int)(num % combination.Length)]);
#else
                    rng.GetBytes(uintBuffer, offset, count);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    if (num < max)
                    {
                        result.Append(combination[(int)(num % combination.Length)]);
                    }
#endif
                }
            }
            return result.ToString();
        }
    }
}
