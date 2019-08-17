using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Asp.Core.Interfaces;

namespace Utilities.Asp.Core.Repository.Boilerplate
{
    public static class Generator
    {
        public static void Generate(IGeneratorStrategy generator)
        {
            generator.Generate();
        }
    }
}
