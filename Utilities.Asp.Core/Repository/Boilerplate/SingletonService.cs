using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Utilities.Asp.Core.Interfaces;
using Utilities.SQL.Generator.Model;

namespace Utilities.Asp.Core.Repository.Boilerplate
{
    public class SingletonServiceGenerator : GenericServiceAbstract<SqlConnection>, IGeneratorStrategy
    {
        public SingletonServiceGenerator(string connectionString, string directory, string @namespace)
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
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using Utilities.Asp.Core.Repository;");
            sb.AppendLine("using System.Data.SqlClient;");
            sb.AppendLine("using Utilities.SQL;");
            sb.AppendLine($"using {Namespace}.Models;");
            sb.AppendLine();
            sb.AppendLine($@"namespace {Namespace}.Repositories");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {repositoryName} : Repository<{tableName}>");
            sb.AppendLine("    {");
            sb.AppendLine($"       public {repositoryName}(DatabaseConnector<SqlConnection,SqlParameter> connector) : base(connector)");
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
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using Utilities.Asp.Core.Repository;");
            sb.AppendLine("using System.Data.SqlClient;");
            sb.AppendLine("using Utilities.SQL;");
            sb.AppendLine("using Utilities.Asp.Core.Repository.Interfaces;");
            sb.AppendLine("using System.Data.Common;");
            sb.AppendLine($"using {Namespace}.Repositories;");
            sb.AppendLine();
            sb.AppendLine($@"namespace {Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Service : IUnitOfWork");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly static Lazy<Service> _lazyInstant = new Lazy<Service>(()=> new Service());");
            sb.AppendLine("        public readonly static Service Context = _lazyInstant.Value;");
            sb.AppendLine("        private readonly DatabaseConnector<SqlConnection,SqlParameter> _connection;");
            sb.AppendLine("        Service()");
            sb.AppendLine("        {");
            sb.AppendLine($"                _connection = new DatabaseConnector<SqlConnection,SqlParameter>(\"{ConnectionString}\");");
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
            var outputPath = Path.Combine(Directory, "Service.cs");
            System.IO.File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
        }
    }
}
