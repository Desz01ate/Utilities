using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Utilities.Security.Cryptography
{
    /// <summary>
    /// Simple wrapper for Rivest–Shamir–Adleman method
    /// </summary>
    public static class RSA
    {
        /// <summary>
        /// Generate private key and public key as a pair.
        /// </summary>
        /// <returns>Tuple of private and public key</returns>
        public static (string PrivateKey, string PublicKey) KeyGenerator()
        {
            var privateKey = CreatePrivateKey();
            var publicKey = CreatePublicKey(privateKey);
            return (privateKey, publicKey);
        }
        /// <summary>
        /// Generate private key.
        /// </summary>
        /// <returns>private key as string</returns>
        public static string CreatePrivateKey()
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(1024, cspParams);

            string privateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));

            return privateKey;
        }
        /// <summary>
        /// Generate public key using private key.
        /// </summary>
        /// <param name="privateKey">Private key as string</param>
        /// <returns>public key as string</returns>
        public static string CreatePublicKey(string privateKey)
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);

            rsaProvider.ImportCspBlob(Convert.FromBase64String(privateKey));

            string publicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));

            return publicKey;
        }
        /// <summary>
        /// Encrypt raw data using public key
        /// </summary>
        /// <param name="publicKey">Public key as string</param>
        /// <param name="data">Raw data to encrypt</param>
        /// <returns>Encrypted data as byte array</returns>
        public static byte[] Encrypt(string publicKey, string data)
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);
            rsaProvider.ImportCspBlob(Convert.FromBase64String(publicKey));
            byte[] plainBytes = Encoding.UTF8.GetBytes(data);
            byte[] encryptedBytes = rsaProvider.Encrypt(plainBytes, false);
            return encryptedBytes;
        }
        /// <summary>
        /// Decrypt encrypted byte array using private key
        /// </summary>
        /// <param name="privateKey">Private key as string</param>
        /// <param name="encryptedBytes">Encrypted byte array</param>
        /// <returns>Raw data of encrypted byte array</returns>
        public static string Decrypt(string privateKey, byte[] encryptedBytes)
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);
            rsaProvider.ImportCspBlob(Convert.FromBase64String(privateKey));
            byte[] plainBytes = rsaProvider.Decrypt(encryptedBytes, false);
            string plainText = Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);
            return plainText;
        }

    }
}
