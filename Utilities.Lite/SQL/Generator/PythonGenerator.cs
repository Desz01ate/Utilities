using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using Utilities.SQL.Generator;
using Utilities.SQL.Generator.Model;

namespace Utilities.SQL.Generator
{
    public class PythonGenerator<TDatabase> : AbstractModelGenerator<TDatabase>
        where TDatabase : DbConnection, new()
    {
        public PythonGenerator(string connectionString, string directory, string @namespace) : base(connectionString, directory, @namespace)
        {
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
    }
}
