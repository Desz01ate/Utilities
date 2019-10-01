using ModelGenerator.Services.DesignPattern.Interfaces;
using ModelGenerator.Services.Generator;
using ModelGenerator.Services.Generator.Model;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using Utilities.Interfaces;

namespace ModelGenerator.Services.DesignPattern.UnitOfWork.Strategy.NonSingleton
{
    public class VisualBasicNonSingletonStrategy<TDatabase> : IGeneratorStrategy<TDatabase> where TDatabase : DbConnection, new()
    {
        public VisualBasicNonSingletonStrategy(string connectionString, string directory, string @namespace)
        {
            ConnectionString = connectionString;
            Directory = directory;
            Namespace = @namespace;
            Generator = new VisualBasicGenerator<TDatabase>(connectionString, ModelDirectory, $"{@namespace}");
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
            sb.AppendLine($"Imports System.Data.SqlClient");
            sb.AppendLine($"Imports Utilities.Interfaces");
            sb.AppendLine($"Imports Utilities.DesignPattern.UnitOfWork.Components");
            sb.AppendLine($"Imports {Namespace}.Models");
            sb.AppendLine();
            sb.AppendLine($@"Namespace {Namespace}.Repositories");
            sb.AppendLine($"    public Class {repositoryName}");
            sb.AppendLine($"    Inherits Repository(Of {tableName},SqlConnection,SqlParameter)");
            sb.AppendLine($"       Public Sub New(connector As IDatabaseConnectorExtension(Of SqlConnection,SqlParameter))");
            sb.AppendLine($"           MyBase.New(connector)");
            sb.AppendLine($"       End Sub");
            sb.AppendLine("    End Class");
            sb.AppendLine("End Namespace");
            var outputFile = Path.Combine(RepositoryDirectory, $"{repositoryName}.vb");
            System.IO.File.WriteAllText(outputFile, sb.ToString(), Encoding.UTF8);
        }

        public void GenerateService()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Imports System");
            sb.AppendLine("Imports System.Data.SqlClient");
            sb.AppendLine("Imports Utilities.SQL");
            sb.AppendLine("Imports System.Data.Common");
            sb.AppendLine("Imports Utilities.Interfaces");
            sb.AppendLine($"Imports {Namespace}.Repositories");
            sb.AppendLine();
            sb.AppendLine($@"Namespace {Namespace}");
            sb.AppendLine($"    Public NotInheritable Class Service");
            sb.AppendLine($"                                Implements IUnitOfWork");
            sb.AppendLine("        Private _connection As IDatabaseConnectorExtension(Of SqlConnection,SqlParameter)");
            sb.AppendLine("        Public Sub New(connector As IDatabaseConnectorExtension(Of SqlConnection,SqlParameter))");
            sb.AppendLine($"                _connection = connector");
            sb.AppendLine("        End Sub");
            sb.AppendLine("        Public Sub New()");
            sb.AppendLine($"                _connection = new DatabaseConnector(Of SqlConnection,SqlParameter)(\"***YOUR DATABASE CREDENTIAL***\")");
            sb.AppendLine("        End Sub");
            foreach (var table in Generator.Tables)
            {
                var tableName = TableNameCleanser(table);
                var repositoryName = $"{tableName}Repository";
                sb.AppendLine($"        Private  _{tableName} As {repositoryName}");
                sb.AppendLine($"        Public ReadOnly Property {tableName}() As {repositoryName}");
                sb.AppendLine($"            Get");
                sb.AppendLine($"                If(_{tableName} Is Nothing) Then");
                sb.AppendLine($"                    _{tableName} = new {repositoryName}(_connection)");
                sb.AppendLine($"                End If");
                sb.AppendLine($"                return _{tableName}");
                sb.AppendLine($"            End Get");
                sb.AppendLine($"        End Property");
            }
            var unitOfWorkMethods = typeof(IUnitOfWork).GetMethods();
            foreach (var method in unitOfWorkMethods)
            {
                var parameters = string.Join(",", method.GetParameters().Select(parameter =>
                {
                    return $"{parameter.Name} As {parameter.ParameterType.Name}";
                }));
                var returnType = method.ReturnType == typeof(void) ? "" : $"As {method.ReturnType.Name}";
                var functionType = method.ReturnType == typeof(void) ? "Sub" : "Function";
                var methodName = method.Name;
                sb.AppendLine($"            Public {functionType} {methodName}({parameters}) {returnType} Implements IUnitOfWork.{methodName}");
                sb.AppendLine($"                       throw new NotImplementedException()");
                sb.AppendLine($"            End {functionType}");
            }
            sb.AppendLine($"            Public Sub Dispose() Implements IDisposable.Dispose");
            sb.AppendLine($"                _connection.Dispose()");
            sb.AppendLine("             End Sub");
            sb.AppendLine("    End Class");
            sb.AppendLine("End Namespace");
            var outputPath = Path.Combine(Directory, "Service.vb");
            System.IO.File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
        }
    }
}
