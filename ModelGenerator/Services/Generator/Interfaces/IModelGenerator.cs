using ModelGenerator.Services.Generator.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGenerator.Services.Generator.Interfaces
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
