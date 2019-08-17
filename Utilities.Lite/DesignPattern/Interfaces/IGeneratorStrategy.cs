using System;
using System.Collections.Generic;
using System.Text;
using Utilities.SQL.Generator;

namespace Utilities.Interfaces
{
    public interface IGeneratorStrategy
    {
        string Directory { get; }
        string Namespace { get; }
        string ConnectionString { get; }
        void Generate();
    }
}
