using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Utilities.Asp.Core.Enumerables;
using Utilities.SQL.Generator;
using Utilities.SQL.Generator.Model;

namespace Utilities.Asp.Core.Repository.Boilerplate
{
    public static class BoilerplateGenerator
    {
        /// <summary>
        /// Generate Repository Pattern Unit Of Work (currently supported just SQL Server + C#)
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="outputPath"></param>
        /// <param name="targetNamespace"></param>
        public static void GenerateRepositoryService(string connectionString, string outputDirectory, string targetNamespace, RepositoryType repositoryType = RepositoryType.NonSingleton)
        {
            var modelDirectory = Path.Combine(outputDirectory, "Models");
            var repositoryDirectory = Path.Combine(outputDirectory, "Repositories");
            Directory.CreateDirectory(modelDirectory);
            Directory.CreateDirectory(repositoryDirectory);

            var generator = new Utilities.SQL.Generator.ModelGenerator<SqlConnection>(connectionString, modelDirectory, $"{targetNamespace}.Models");
            generator.GenerateAllTables(SQL.Generator.Enumerable.TargetLanguage.CSharp);
            GenerateRepositories(generator, repositoryDirectory, targetNamespace);
            switch (repositoryType)
            {
                case RepositoryType.Singleton:
                    GenerateSingletonService(generator, connectionString, outputDirectory, targetNamespace);
                    break;
                case RepositoryType.NonSingleton:
                    GenerateNonSingletonService(generator, connectionString, outputDirectory, targetNamespace);
                    break;
            }
        }

        private static void GenerateRepositories(ModelGenerator<SqlConnection> generator, string repositryDirectory, string targetNamespace)
        {
            //var files = Directory.EnumerateFiles(modelDirectory);
            foreach (var table in generator.Tables)
            {
                generator.GenerateFromTable(table, (tb) => GenerateRepository(tb, repositryDirectory, targetNamespace));
            }
        }
        private static string TableNameCleanser(string tableName)
        {
            return tableName.Replace("-", "");
        }
        private static void GenerateRepository(Table table, string repositoryDirectory, string targetNamespace)
        {
            var tableName = TableNameCleanser(table.Name);
            var repositoryName = $"{tableName}Repository";
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using Utilities.Asp.Core.Repository;");
            sb.AppendLine("using System.Data.SqlClient;");
            sb.AppendLine("using Utilities.SQL;");
            sb.AppendLine($"using {targetNamespace}.Models;");
            sb.AppendLine();
            sb.AppendLine($@"namespace {targetNamespace}.Repositories");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {repositoryName} : Repository<{tableName}>");
            sb.AppendLine("    {");
            sb.AppendLine($"       public {repositoryName}(DatabaseConnector<SqlConnection,SqlParameter> connector) : base(connector)");
            sb.AppendLine($"       {{");
            sb.AppendLine($"       }}");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            var outputFile = Path.Combine(repositoryDirectory, $"{repositoryName}.cs");
            System.IO.File.WriteAllText(outputFile, sb.ToString(), Encoding.UTF8);
        }
        private static void GenerateSingletonService(ModelGenerator<SqlConnection> generator, string connectionString, string outputDirectory, string targetNamespace)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using Utilities.Asp.Core.Repository;");
            sb.AppendLine("using System.Data.SqlClient;");
            sb.AppendLine("using Utilities.SQL;");
            sb.AppendLine("using Utilities.Asp.Core.Repository.Interfaces;");
            sb.AppendLine("using System.Data.Common;");
            sb.AppendLine($"using {targetNamespace}.Repositories;");
            sb.AppendLine();
            sb.AppendLine($@"namespace {targetNamespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Service : IUnitOfWork");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly static Lazy<Service> _lazyInstant = new Lazy<Service>(()=> new Service());");
            sb.AppendLine("        public readonly static Service Context = _lazyInstant.Value;");
            sb.AppendLine("        private readonly DatabaseConnector<SqlConnection,SqlParameter> _connection;");
            sb.AppendLine("        Service()");
            sb.AppendLine("        {");
            sb.AppendLine($"                _connection = new DatabaseConnector<SqlConnection,SqlParameter>(\"{connectionString}\");");
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
            sb.AppendLine($"            public DbTransaction BeginTransaction()");
            sb.AppendLine("            {");
            sb.AppendLine($"                return _connection.BeginTransaction();");
            sb.AppendLine("            }");
            sb.AppendLine($"            public void SaveChanges(DbTransaction transaction)");
            sb.AppendLine("            {");
            sb.AppendLine($"                transaction?.Commit();");
            sb.AppendLine("            }");
            sb.AppendLine($"            public void RollbackChanges(DbTransaction transaction)");
            sb.AppendLine("            {");
            sb.AppendLine($"                transaction?.Rollback();");
            sb.AppendLine("            }");
            sb.AppendLine($"            public void Dispose()");
            sb.AppendLine("            {");
            sb.AppendLine($"                _connection.Dispose();");
            sb.AppendLine("            }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            var outputPath = Path.Combine(outputDirectory, "Service.cs");
            System.IO.File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
        }
        private static void GenerateNonSingletonService(ModelGenerator<SqlConnection> generator, string connectionString, string outputDirectory, string targetNamespace)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using Utilities.Asp.Core.Repository;");
            sb.AppendLine("using System.Data.SqlClient;");
            sb.AppendLine("using Utilities.SQL;");
            sb.AppendLine("using Utilities.Asp.Core.Repository.Interfaces;");
            sb.AppendLine("using System.Data.Common;");
            sb.AppendLine($"using {targetNamespace}.Repositories;");
            sb.AppendLine();
            sb.AppendLine($@"namespace {targetNamespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Service : IUnitOfWork");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly DatabaseConnector<SqlConnection,SqlParameter> _connection;");
            sb.AppendLine("        public Service(DatabaseConnector<SqlConnection,SqlParameter> connector)");
            sb.AppendLine("        {");
            sb.AppendLine($"                _connection = connector;");
            sb.AppendLine("        }");
            sb.AppendLine("        public Service()");
            sb.AppendLine("        {");
            sb.AppendLine($"                _connection = new DatabaseConnector<SqlConnection,SqlParameter>(\"{connectionString}\");");
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
            sb.AppendLine($"            public DbTransaction BeginTransaction()");
            sb.AppendLine("            {");
            sb.AppendLine($"                return _connection.BeginTransaction();");
            sb.AppendLine("            }");
            sb.AppendLine($"            public void SaveChanges(DbTransaction transaction)");
            sb.AppendLine("            {");
            sb.AppendLine($"                transaction?.Commit();");
            sb.AppendLine("            }");
            sb.AppendLine($"            public void RollbackChanges(DbTransaction transaction)");
            sb.AppendLine("            {");
            sb.AppendLine($"                transaction?.Rollback();");
            sb.AppendLine("            }");
            sb.AppendLine($"            public void Dispose()");
            sb.AppendLine("            {");
            sb.AppendLine($"                _connection.Dispose();");
            sb.AppendLine("            }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            var outputPath = Path.Combine(outputDirectory, "Service.cs");
            System.IO.File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
        }

    }
}
