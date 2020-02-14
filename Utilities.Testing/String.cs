using NUnit.Framework;

namespace Utilities.Testing
{
    [TestFixture]
    internal class String
    {
        [Test]
        public void LeadingUpper()
        {
            var case1 = "helloworld";
            var case2 = "hello world";
            var case3 = "this is a string, I do not want to work with it!";
            var result1 = Utilities.String.ToLeadingUpper(case1);
            Assert.AreEqual("Helloworld", result1);
            var result2 = Utilities.String.ToLeadingUpper(case2);
            Assert.AreEqual("Hello world", result2);
            var result3 = Utilities.String.ToLeadingUpper(case3);
            Assert.AreEqual("This is a string, I do not want to work with it!", result3);
            Assert.Pass();
        }

        [Test]
        public void LeadingUpper_AllWords()
        {
            var case1 = "helloworld";
            var case2 = "hello world";
            var case3 = "this is a string, I do not want to work with it!";
            var result1 = Utilities.String.ToLeadingUpper(case1, true);
            Assert.AreEqual("Helloworld", result1);
            var result2 = Utilities.String.ToLeadingUpper(case2, true);
            Assert.AreEqual("Hello World", result2);
            var result3 = Utilities.String.ToLeadingUpper(case3, true);
            Assert.AreEqual("This Is A String, I Do Not Want To Work With It!", result3);
            Assert.Pass();
        }

        [Test]
        public void LeadingUpper_AllWords_CustomSeperator()
        {
            var case1 = "helloworld";
            var case2 = "hello,world";
            var case3 = "this,is,a,string, I,do,not,want,to,work,with,it!";
            var result1 = Utilities.String.ToLeadingUpper(case1, true, ',');
            Assert.AreEqual("Helloworld", result1);
            var result2 = Utilities.String.ToLeadingUpper(case2, true, ',');
            Assert.AreEqual("Hello,World", result2);
            var result3 = Utilities.String.ToLeadingUpper(case3, true, ',');
            Assert.AreEqual("This,Is,A,String, I,Do,Not,Want,To,Work,With,It!", result3);
            Assert.Pass();
        }
    }
}