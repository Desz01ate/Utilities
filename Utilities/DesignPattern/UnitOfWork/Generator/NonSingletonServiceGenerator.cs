using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using Utilities.Interfaces;
using Utilities.SQL.Generator.Model;

namespace Utilities.DesignPattern.UnitOfWork
{
    /// <summary>
    /// Generator for Non-Singleton Unit Of Work boilerplate
    /// </summary>
    /// <typeparam name="TDatabase"></typeparam>
    public sealed class NonSingletonServiceGenerator<TDatabase> : GenericServiceAbstract<TDatabase>, IGeneratorStrategy
        where TDatabase : DbConnection, new()
    {
        public NonSingletonServiceGenerator(string connectionString, string directory, string @namespace)
        {
            ConnectionString = connectionString;
            Directory = directory;
            Namespace = @namespace;
        }
        protected override void GenerateRepository(Table tb)
        {
            var tableName = TableNameCleanser(tb.Name);
            var repositoryName = $"{tableName}Repository";
            var sb = new StringBuilder();
            sb.AppendLine($"using System.Data.SqlClient;");
            sb.AppendLine($"using Utilities.Interfaces;");
            sb.AppendLine($"using Utilities.DesignPattern.UnitOfWork.Components;");
            sb.AppendLine($"using {Namespace}.Models;");
            sb.AppendLine();
            sb.AppendLine($@"namespace {Namespace}.Repositories");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {repositoryName} : Repository<{tableName},SqlConnection,SqlParameter>");
            sb.AppendLine("    {");
            sb.AppendLine($"       public {repositoryName}(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)");
            sb.AppendLine($"       {{");
            sb.AppendLine($"       }}");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            var outputFile = Path.Combine(RepositoryDirectory, $"{repositoryName}.cs");
            System.IO.File.WriteAllText(outputFile, sb.ToString(), Encoding.UTF8);
        }
        protected override void GenerateService()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Data.SqlClient;");
            sb.AppendLine("using Utilities.SQL;");
            sb.AppendLine("using System.Data.Common;");
            sb.AppendLine("using Utilities.Interfaces;");
            sb.AppendLine($"using {Namespace}.Repositories;");
            sb.AppendLine();
            sb.AppendLine($@"namespace {Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public sealed class Service : IUnitOfWork");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly IDatabaseConnectorExtension<SqlConnection,SqlParameter> _connection;");
            sb.AppendLine("        public Service(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector)");
            sb.AppendLine("        {");
            sb.AppendLine($"                _connection = connector;");
            sb.AppendLine("        }");
            sb.AppendLine("        public Service()");
            sb.AppendLine("        {");
            sb.AppendLine($"                _connection = new DatabaseConnector<SqlConnection,SqlParameter>(\"***YOUR DATABASE CREDENTIAL***\");");
            sb.AppendLine("        }");
            foreach (var table in generator.Tables)
            {
                var tableName = TableNameCleanser(table);
                var repositoryName = $"{tableName}Repository";
                sb.AppendLine($"        private {repositoryName} _{tableName} {{ get; set; }}");
                sb.AppendLine($"        public {repositoryName} {tableName}");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            get");
                sb.AppendLine($"            {{");
                sb.AppendLine($"                if(_{tableName} == null)");
                sb.AppendLine($"                {{");
                sb.AppendLine($"                    _{tableName} = new {repositoryName}(_connection);");
                sb.AppendLine($"                }}");
                sb.AppendLine($"                return _{tableName};");
                sb.AppendLine($"            }}");
                sb.AppendLine($"        }}");
            }
            var unitOfWorkMethods = typeof(IUnitOfWork).GetMethods();
            foreach (var method in unitOfWorkMethods)
            {
                var parameters = string.Join(",", method.GetParameters().Select(parameter =>
                 {
                     return $"{parameter.ParameterType.Name} {parameter.Name}";
                 }));
                var returnType = method.ReturnType == typeof(void) ? "void" : method.ReturnType.Name;
                var methodName = method.Name;
                sb.AppendLine($"            public {returnType} {methodName}({parameters}) => throw new NotImplementedException();");
            }
            sb.AppendLine($"            public void Dispose()");
            sb.AppendLine("            {");
            sb.AppendLine($"                _connection.Dispose();");
            sb.AppendLine("            }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            var outputPath = Path.Combine(Directory, "Service.cs");
            System.IO.File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
        }
    }
}
