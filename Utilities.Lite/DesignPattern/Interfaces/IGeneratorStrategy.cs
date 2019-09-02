using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Utilities.SQL.Generator;
using Utilities.SQL.Generator.Model;

namespace Utilities.Interfaces
{
    public interface IGeneratorStrategy<TDatabase> where TDatabase : DbConnection, new()
    {
        string Directory { get; }
        string Namespace { get; }
        string ConnectionString { get; }
        string ModelDirectory { get; }
        string RepositoryDirectory { get; }
        AbstractModelGenerator<TDatabase> Generator { get; }
        void GenerateModel();
        void GenerateRepository(Table table);
        void GenerateService();
    }
}
