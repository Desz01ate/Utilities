using System;
using System.Collections.Generic;
using System.Text;
using Utilities.SQL.Generator.Model;

namespace Utilities.Interfaces
{
    public interface IModelGenerator
    {
        string ConnectionString { get; }
        string Directory { get; }
        string Namespace { get; }
        List<string> Tables { get; }
        void GenerateAllTable();
        void GenerateFromSpecificTable(string tableName, Action<Table> parser);
    }
}
