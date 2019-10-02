﻿using ModelGenerator.Services.DesignPattern.Interfaces;
using ModelGenerator.Services.Generator;
using ModelGenerator.Services.Generator.Model;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using Utilities.Interfaces;

namespace ModelGenerator.Services.DesignPattern.UnitOfWork.Strategy.NonSingleton
{
    public class CSharpNonSingletonStrategy<TDatabase> : IGeneratorStrategy<TDatabase> where TDatabase : DbConnection, new()
    {
        public CSharpNonSingletonStrategy(string connectionString, string directory, string @namespace)
        {
            ConnectionString = connectionString;
            Directory = directory;
            Namespace = @namespace;
            Generator = new CSharpGenerator<TDatabase>(connectionString, ModelDirectory, $"{@namespace}.Models");
        }
        public string Directory { get; }

        public string Namespace { get; }

        public string ConnectionString { get; }

        public string ModelDirectory => Path.Combine(Directory, "Models");

        public string RepositoryDirectory => Path.Combine(Directory, "Repositories");

        public AbstractModelGenerator<TDatabase> Generator { get; }
        private string TableNameCleanser(string tableName)
        {
            return tableName.Replace("-", "");
        }

        public void GenerateModel()
        {
            Generator.GenerateAllTable();
        }

        public void GenerateRepository(Table table)
        {
            var tableName = TableNameCleanser(table.Name);
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

        public void GenerateService()
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
            foreach (var table in Generator.Tables)
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