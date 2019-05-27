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
    }
}