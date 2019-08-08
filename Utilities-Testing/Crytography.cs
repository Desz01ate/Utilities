using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Testing
{
    [TestFixture]
    class Crytography
    {
        [Test]
        public void EncAndDec()
        {
            var plainText = "19500";
            var salt = "ISUSTAE";
            var blockSizes = new[] { 128 }; //256 bit entropy is not working atm.
            foreach (var block in blockSizes)
            {
                for (var iter = 2000; iter < 10000; iter += 1000)
                {
                    var saltBytes = Utilities.Cryptography.GenerateSalt();
                    var encrypt = Utilities.Cryptography.Encrypt(plainText, salt, block, iter);
                    var decrypt = Utilities.Cryptography.Decrypt(encrypt, salt, block, iter);
                    var gh1 = Utilities.Cryptography.GenerateHash(plainText, 128, iter);
                    var gh2 = Utilities.Cryptography.GenerateHash(plainText, saltBytes, 128, iter);
                    var gh3 = Utilities.Cryptography.GenerateHash(plainText, Convert.ToBase64String(saltBytes), 128, iter);
                    Assert.AreEqual(decrypt, plainText);
                    Assert.IsTrue(Utilities.Cryptography.Verify(plainText,  gh1.Hash, gh1.Salt, iter));
                    Assert.IsTrue(Utilities.Cryptography.Verify(plainText,  gh2.Hash, gh2.Salt, iter));
                    Assert.IsTrue(Utilities.Cryptography.Verify(plainText,  gh3.Hash, gh3.Salt, iter));
                    Assert.IsFalse(Utilities.Cryptography.Verify(plainText, gh1.Hash, gh1.Salt, iter + 1));
                    Assert.IsFalse(Utilities.Cryptography.Verify(plainText, gh2.Hash, gh2.Salt, iter + 1));
                    Assert.IsFalse(Utilities.Cryptography.Verify(plainText, gh3.Hash, gh3.Salt, iter + 1));
                    Assert.IsFalse(Utilities.Cryptography.Verify(plainText, gh1.Hash, gh2.Salt, iter));
                    //since gh2,gh3 use the same salt while gh1 randomly generatH new one,Swe use 1 as a false comparer.
                    Assert.IsFalse(Utilities.Cryptography.Verify(plainText, gh2.Hash, gh1.Salt, iter));
                    Assert.IsFalse(Utilities.Cryptography.Verify(plainText, gh3.Hash, gh1.Salt, iter));
                    Assert.IsFalse(Utilities.Cryptography.Verify(plainText, gh1.Hash, gh2.Salt, iter + 1));
                    Assert.IsFalse(Utilities.Cryptography.Verify(plainText, gh2.Hash, gh3.Salt, iter + 1));
                    Assert.IsFalse(Utilities.Cryptography.Verify(plainText, gh3.Hash, gh1.Salt, iter + 1));
                }
            }


            Assert.Pass();
        }
    }
}
