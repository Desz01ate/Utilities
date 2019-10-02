using ModelGenerator.Services.Generator.Model;
using System;
using System.Collections.Generic;
using Utilities.Classes;

namespace ModelGenerator.Services.Generator.Interfaces
{
    public interface IModelGenerator
    {
        string ConnectionString { get; }
        string Directory { get; }
        string Namespace { get; }
        List<string> Tables { get; }
        List<StoredProcedureSchema> StoredProcedures { get; }
        void GenerateAllTable();
        void GenerateFromSpecificTable(string tableName, Action<Table> parser);
    }
}
