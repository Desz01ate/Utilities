using NUnit.Framework;

namespace Utilities.Testing
{
    [TestFixture]
    public class RegularExpression
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PhoneNumber()
        {
            var phoneNumTrue1 = Utilities.RegularExpression.IsPhoneNumber("0831234567");
            Assert.IsTrue(phoneNumTrue1);
            var phoneNumTrue2 = Utilities.RegularExpression.IsPhoneNumber("+66831234567");
            Assert.IsTrue(phoneNumTrue2);
            //length is equal to 8
            for (var i = 0; i < 10; i++)
            {
                Assert.IsTrue(Utilities.RegularExpression.IsPhoneNumber($@"08{i.ToString().PadLeft(8, '0')}"));
            }
            //length < 8
            for (var i = 0; i < 10; i++)
            {
                Assert.IsFalse(Utilities.RegularExpression.IsPhoneNumber($@"08{i.ToString().PadLeft(7, '0')}"));
            }
            //length > 8
            for (var i = 0; i < 10; i++)
            {
                var result = Utilities.RegularExpression.IsPhoneNumber($@"08{i.ToString().PadLeft(10, '0')}");
                Assert.IsFalse(result);
            }
            Assert.Pass();
        }

        [Test]
        public void Email()
        {
            var emailTrue = Utilities.RegularExpression.IsEmail("test@test.com");
            var emailTrue2 = Utilities.RegularExpression.IsEmail("test!#$%@test.com");
            var emailFalse = Utilities.RegularExpression.IsEmail("test@test");
            var emailFalse2 = Utilities.RegularExpression.IsEmail("test!#$%@.com");
            var emailFalse3 = Utilities.RegularExpression.IsEmail("test!#$%@!test.com");
            Assert.IsTrue(emailTrue);
            Assert.IsTrue(emailTrue2);
            Assert.IsFalse(emailFalse);
            Assert.IsFalse(emailFalse2);
            Assert.IsFalse(emailFalse3);
            Assert.Pass();
        }

        [Test]
        public void Digit()
        {
            for (var i = 0; i < 1000; i++)
            {
                Assert.IsTrue(Utilities.RegularExpression.IsOnlyDigit(i.ToString()));
                if (48 <= i && i <= 57) continue; // 48 to 57 is character of digits
                var asc = (char)i;
                Assert.IsFalse(Utilities.RegularExpression.IsOnlyDigit(asc.ToString()));
            }
            Assert.Pass();
        }

        [Test]
        public void Text()
        {
            for (var i = 65; i < 91; i++)
            {
                string asc = ((char)i).ToString();
                Assert.IsTrue(Utilities.RegularExpression.IsOnlyText(asc));
                Assert.IsFalse(Utilities.RegularExpression.IsOnlyText(i.ToString()));
                Assert.IsFalse(Utilities.RegularExpression.IsOnlyText($@"{asc}{i}"));
            }
            Assert.IsTrue(Utilities.RegularExpression.IsOnlyText("สวัสดี"));
            Assert.Pass();
        }

        [Test]
        public void CitizenId_Length13_ValidCase()
        {
            var valid = new[] {
                "5051485224650",
                "3945055097147",
                "3997063468789",
                "1016991739632",
                "3511798193086",
            };
            foreach (var test in valid)
            {
                var result = Utilities.RegularExpression.IsValidThaiCitizenId(test);
                Assert.AreEqual(true, result);
            }
            Assert.Pass();
        }

        [Test]
        public void CitizenId_Length13_InvalidCase()
        {
            var valid = new[] {
                "5051485224652",
                "3945055097142",
                "3997063468782",
                "1016991739123",
                "1234567890123"
            };
            foreach (var test in valid)
            {
                var result = Utilities.RegularExpression.IsValidThaiCitizenId(test);
                Assert.AreEqual(false, result);
            }
            Assert.Pass();
        }

        [Test]
        public void CitizenId_NullEmpty_InvalidCase()
        {
            var result1 = Utilities.RegularExpression.IsValidThaiCitizenId(null);
            var result2 = Utilities.RegularExpression.IsValidThaiCitizenId("");
            Assert.AreEqual(false, result1);
            Assert.AreEqual(false, result2);
            Assert.Pass();
        }

        [Test]
        public void CitizenId_LengthNotEqualTo13_InvalidCase()
        {
            var valid = new[] {
                "505148522465",
                "39450550971",
                "3997063468",
                "101699173",
                "12345678901233"
            };
            foreach (var test in valid)
            {
                var result = Utilities.RegularExpression.IsValidThaiCitizenId(test);
                Assert.AreEqual(false, result);
            }
            Assert.Pass();
        }
    }
}