using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using Utilities.SQL.Generator;
using Utilities.SQL.Generator.Model;

namespace Utilities.SQL.Generator
{
    public class PHPGenerator<TDatabase> : AbstractModelGenerator<TDatabase>
        where TDatabase : DbConnection, new()
    {
        public PHPGenerator(string connectionString, string directory, string @namespace) : base(connectionString, directory, @namespace)
        {
        }

        protected override void GenerateCodeFile(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<?php");
            sb.AppendLine($"class {table.Name}");
            sb.AppendLine("{");
            foreach (var column in table.Columns)
            {
                var col = ColumnNameCleanser(column.ColumnName);
                sb.AppendLine($"    var ${col};");
                sb.AppendLine($"    function get{col}(){{");
                sb.AppendLine($"        return $this->{col};");
                sb.AppendLine($"    }}");
                sb.AppendLine($"    function set{col}($value){{");
                sb.AppendLine($"        $this->0h{col} = $value;");
                sb.AppendLine($"    }}");
            }
            sb.AppendLine("}");
            sb.AppendLine("?>");
            var filePath = Path.Combine(Directory, $@"{table.Name}.php");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
    }
}
