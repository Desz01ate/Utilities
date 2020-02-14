using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Numerics;

namespace Utilities.Testing
{
    [TestFixture]
    class Numerics
    {
        [Test]
        public void DigitLength()
        {
            int iv = 10000000;
            double dv = 10000000;
            float fv = 10000000;
            decimal mv = 10000000;
            var expectedLength = 8;

            var ir = iv.Length();
            var dr = dv.Length();
            var fr = fv.Length();
            var mr = mv.Length();
            Assert.AreEqual(expectedLength, ir);
            Assert.AreEqual(expectedLength, dr);
            Assert.AreEqual(expectedLength, fr);
            Assert.AreEqual(expectedLength, mr);
        }
    }
}
