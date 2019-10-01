using ModelGenerator.Services.DesignPattern.UnitOfWork.Generator;
using ModelGenerator.Services.DesignPattern.UnitOfWork.Strategy.NonSingleton;
using ModelGenerator.Services.Generator;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.ComponentModel;
using System.Data.SqlClient;

namespace ModelGenerator.Enum
{
    public enum TargetLanguage
    {
        [Description("C#")]
        CSharp = 0,
        [Description("Visual Basic")]
        VisualBasic = 1,
        [Description("TypeScript")]
        TypeScript = 2,
        [Description("PHP")]
        PHP = 3,
        [Description("Python")]
        Python = 4,
        [Description("Python 3.7")]
        Python37 = 5,
        [Description("Java")]
        Java = 6,
        [Description("C++")]
        CPP = 7
    }
    public static class LangaugesData
    {
        public static void PerformModelGenerate(TargetLanguage targetLanguage, TargetDatabaseConnector targetDatabaseConnector, string connectionString, string directory, string @namespace)
        {
            switch (targetLanguage)
            {
                case TargetLanguage.CSharp:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            new CSharpGenerator<SqlConnection>(connectionString, directory, @namespace, (x) => $"[{x}]").GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.Oracle:
                            new CSharpGenerator<OracleConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.MySQL:
                            new CSharpGenerator<MySqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.PostgreSQL:
                            new CSharpGenerator<NpgsqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                    }
                    break;
                case TargetLanguage.VisualBasic:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            new VisualBasicGenerator<SqlConnection>(connectionString, directory, @namespace, (x) => $"[{x}]").GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.Oracle:
                            new VisualBasicGenerator<OracleConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.MySQL:
                            new VisualBasicGenerator<MySqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.PostgreSQL:
                            new VisualBasicGenerator<NpgsqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                    }
                    break;
                case TargetLanguage.TypeScript:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            new TypeScriptGenerator<SqlConnection>(connectionString, directory, @namespace, (x) => $"[{x}]").GenerateAllTable();

                            return;
                        case TargetDatabaseConnector.Oracle:
                            new TypeScriptGenerator<OracleConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.MySQL:
                            new TypeScriptGenerator<MySqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.PostgreSQL:
                            new TypeScriptGenerator<NpgsqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                    }
                    break;
                case TargetLanguage.PHP:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            new PHPGenerator<SqlConnection>(connectionString, directory, @namespace, (x) => $"[{x}]").GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.Oracle:
                            new PHPGenerator<OracleConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.MySQL:
                            new PHPGenerator<MySqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.PostgreSQL:
                            new PHPGenerator<NpgsqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                    }
                    break;
                case TargetLanguage.Python:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            new PythonGenerator<SqlConnection>(connectionString, directory, @namespace, (x) => $"[{x}]").GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.Oracle:
                            new PythonGenerator<OracleConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.MySQL:
                            new PythonGenerator<MySqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.PostgreSQL:
                            new PythonGenerator<NpgsqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                    }
                    break;
                case TargetLanguage.Python37:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            new Python37Generator<SqlConnection>(connectionString, directory, @namespace, (x) => $"[{x}]").GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.Oracle:
                            new Python37Generator<OracleConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.MySQL:
                            new Python37Generator<MySqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.PostgreSQL:
                            new Python37Generator<NpgsqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                    }
                    break;
                case TargetLanguage.Java:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            new JavaGenerator<SqlConnection>(connectionString, directory, @namespace, (x) => $"[{x}]").GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.Oracle:
                            new JavaGenerator<OracleConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.MySQL:
                            new JavaGenerator<MySqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.PostgreSQL:
                            new JavaGenerator<NpgsqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                    }
                    break;
                case TargetLanguage.CPP:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            new CPPGenerator<SqlConnection>(connectionString, directory, @namespace, (x) => $"[{x}]").GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.Oracle:
                            new CPPGenerator<OracleConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.MySQL:
                            new CPPGenerator<MySqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                        case TargetDatabaseConnector.PostgreSQL:
                            new CPPGenerator<NpgsqlConnection>(connectionString, directory, @namespace).GenerateAllTable();
                            return;
                    }
                    break;
            }
        }
        public static void PerformStrategyGenerate(TargetLanguage targetLanguage, TargetDatabaseConnector targetDatabaseConnector, string connectionString, string directory, string @namespace)
        {
            switch (targetLanguage)
            {
                case TargetLanguage.CSharp:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            var strategy1 = new CSharpSingletonStrategy<SqlConnection>(connectionString, directory, @namespace, (x) => $"[{x}]");
                            var generator1 = new UnitOfWorkGenerator<SqlConnection>();
                            generator1.UseStrategy(strategy1);
                            generator1.Generate();
                            break;
                        case TargetDatabaseConnector.Oracle:
                            var strategy2 = new CSharpSingletonStrategy<OracleConnection>(connectionString, directory, @namespace);
                            var generator2 = new UnitOfWorkGenerator<OracleConnection>();
                            generator2.UseStrategy(strategy2);
                            generator2.Generate();
                            break;
                        case TargetDatabaseConnector.MySQL:
                            var strategy3 = new CSharpSingletonStrategy<MySqlConnection>(connectionString, directory, @namespace);
                            var generator3 = new UnitOfWorkGenerator<MySqlConnection>();
                            generator3.UseStrategy(strategy3);
                            generator3.Generate();
                            break;
                        case TargetDatabaseConnector.PostgreSQL:
                            var strategy4 = new CSharpSingletonStrategy<NpgsqlConnection>(connectionString, directory, @namespace);
                            var generator4 = new UnitOfWorkGenerator<NpgsqlConnection>();
                            generator4.UseStrategy(strategy4);
                            generator4.Generate();
                            break;
                    }
                    break;
                case TargetLanguage.VisualBasic:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            var strategy1 = new VisualBasicSingletonStrategy<SqlConnection>(connectionString, directory, @namespace, (x) => $"[{x}]");
                            var generator1 = new UnitOfWorkGenerator<SqlConnection>();
                            generator1.UseStrategy(strategy1);
                            generator1.Generate();
                            break;
                        case TargetDatabaseConnector.Oracle:
                            var strategy2 = new VisualBasicSingletonStrategy<OracleConnection>(connectionString, directory, @namespace);
                            var generator2 = new UnitOfWorkGenerator<OracleConnection>();
                            generator2.UseStrategy(strategy2);
                            generator2.Generate();
                            break;
                        case TargetDatabaseConnector.MySQL:
                            var strategy3 = new VisualBasicSingletonStrategy<MySqlConnection>(connectionString, directory, @namespace);
                            var generator3 = new UnitOfWorkGenerator<MySqlConnection>();
                            generator3.UseStrategy(strategy3);
                            generator3.Generate();
                            break;
                        case TargetDatabaseConnector.PostgreSQL:
                            var strategy4 = new VisualBasicSingletonStrategy<NpgsqlConnection>(connectionString, directory, @namespace);
                            var generator4 = new UnitOfWorkGenerator<NpgsqlConnection>();
                            generator4.UseStrategy(strategy4);
                            generator4.Generate();
                            break;
                    }
                    break;
            }
        }
    }
}
