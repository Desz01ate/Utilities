using ModelGenerator.Services.Generator.Model;
using System;
using System.Data.Common;
using System.IO;
using System.Text;
using Utilities.Classes;

namespace ModelGenerator.Services.Generator
{
    public class PythonGenerator<TDatabase> : AbstractModelGenerator<TDatabase>
        where TDatabase : DbConnection, new()
    {
        public PythonGenerator(string connectionString, string directory, string @namespace, Func<string, string> func = null) : base(connectionString, directory, @namespace)
        {
            if (func != null) this.SetCleanser(func);
        }

        protected override string DataTypeMapper(string column)
        {
            throw new System.NotImplementedException();
        }

        protected override void GenerateCodeFile(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"class {table.Name}:");
            sb.AppendLine($@"  def __init__(self):");
            foreach (var column in table.Columns)
            {
                var col = ColumnNameCleanser(column.ColumnName);
                sb.AppendLine($@"    self.{col} = None");
            }
            var filePath = Path.Combine(Directory, $@"{table.Name}.py");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }

        protected override string GetNullableDataType(TableSchema column)
        {
            throw new System.NotImplementedException();
        }
    }
}
