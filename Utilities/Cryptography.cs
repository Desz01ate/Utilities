using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Utilities
{
    //taking the implementation based-on https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp with some different tuning parameter
    public static class Cryptography
    {
        #region Simple Encryption - Decryption Method
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

        public static (string hash, string salt) GenerateHash(string plainText, byte[] salt, int byteSize = 128, int iterations = 2000)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var rfc2898 = new Rfc2898DeriveBytes(plainTextBytes, salt, iterations))
            {
                var hash = rfc2898.GetBytes(byteSize);
                return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
            }
        }
        public static (string hash, string salt) GenerateHash(string plainText, int byteSize = 128, int iterations = 2000)
        {
            return GenerateHash(plainText, GenerateSalt(), byteSize, iterations);
        }
        public static (string hash, string salt) GenerateHash(string plainText, string salt, int byteSize = 128, int iterations = 2000)
        {
            var saltBytes = Convert.FromBase64String(salt);
            return GenerateHash(plainText, saltBytes, byteSize, iterations);
        }
        public static bool Verify(string plainText, string hash, string salt, int iterations = 2000)
        {
            var inputHash = GenerateHash(plainText, salt, iterations: iterations);
            return inputHash.hash == hash;
        }
        #endregion

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
    }
}
