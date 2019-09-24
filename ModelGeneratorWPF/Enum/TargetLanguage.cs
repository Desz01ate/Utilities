using ModelGenerator.Services.DesignPattern.Interfaces;
using ModelGenerator.Services.DesignPattern.UnitOfWork.Generator;
using ModelGenerator.Services.DesignPattern.UnitOfWork.Strategy.NonSingleton;
using ModelGenerator.Services.Generator;
using ModelGenerator.Services.Generator.Interfaces;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace ModelGeneratorWPF.Enum
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
        public static IModelGenerator GetSpecificGenerator(TargetLanguage targetLanguage, TargetDatabaseConnector targetDatabaseConnector, string connectionString, string directory, string @namespace)
        {
            switch (targetLanguage)
            {
                case TargetLanguage.CSharp:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            return new CSharpGenerator<SqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.Oracle:
                            return new CSharpGenerator<OracleConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.MySQL:
                            return new CSharpGenerator<MySqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.PostgreSQL:
                            return new CSharpGenerator<NpgsqlConnection>(connectionString, directory, @namespace);
                    }
                    break;
                case TargetLanguage.VisualBasic:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            return new VisualBasicGenerator<SqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.Oracle:
                            return new VisualBasicGenerator<OracleConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.MySQL:
                            return new VisualBasicGenerator<MySqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.PostgreSQL:
                            return new VisualBasicGenerator<NpgsqlConnection>(connectionString, directory, @namespace);
                    }
                    break;
                case TargetLanguage.TypeScript:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            return new TypeScriptGenerator<SqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.Oracle:
                            return new TypeScriptGenerator<OracleConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.MySQL:
                            return new TypeScriptGenerator<MySqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.PostgreSQL:
                            return new TypeScriptGenerator<NpgsqlConnection>(connectionString, directory, @namespace);
                    }
                    break;
                case TargetLanguage.PHP:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            return new PHPGenerator<SqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.Oracle:
                            return new PHPGenerator<OracleConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.MySQL:
                            return new PHPGenerator<MySqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.PostgreSQL:
                            return new PHPGenerator<NpgsqlConnection>(connectionString, directory, @namespace);
                    }
                    break;
                case TargetLanguage.Python:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            return new PythonGenerator<SqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.Oracle:
                            return new PythonGenerator<OracleConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.MySQL:
                            return new PythonGenerator<MySqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.PostgreSQL:
                            return new PythonGenerator<NpgsqlConnection>(connectionString, directory, @namespace);
                    }
                    break;
                case TargetLanguage.Python37:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            return new Python37Generator<SqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.Oracle:
                            return new Python37Generator<OracleConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.MySQL:
                            return new Python37Generator<MySqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.PostgreSQL:
                            return new Python37Generator<NpgsqlConnection>(connectionString, directory, @namespace);
                    }
                    break;
                case TargetLanguage.Java:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            return new JavaGenerator<SqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.Oracle:
                            return new JavaGenerator<OracleConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.MySQL:
                            return new JavaGenerator<MySqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.PostgreSQL:
                            return new JavaGenerator<NpgsqlConnection>(connectionString, directory, @namespace);
                    }
                    break;
                case TargetLanguage.CPP:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            return new CPPGenerator<SqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.Oracle:
                            return new CPPGenerator<OracleConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.MySQL:
                            return new CPPGenerator<MySqlConnection>(connectionString, directory, @namespace);
                        case TargetDatabaseConnector.PostgreSQL:
                            return new CPPGenerator<NpgsqlConnection>(connectionString, directory, @namespace);
                    }
                    break;
            }
            return null;
        }
        public static void PerformStrategyGenerate(TargetLanguage targetLanguage, TargetDatabaseConnector targetDatabaseConnector, string connectionString, string directory, string @namespace)
        {
            switch (targetLanguage)
            {
                case TargetLanguage.CSharp:
                    switch (targetDatabaseConnector)
                    {
                        case TargetDatabaseConnector.SQLServer:
                            var strategy1 = new CSharpSingletonStrategy<SqlConnection>(connectionString, directory, @namespace);
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
                            var strategy1 = new VisualBasicSingletonStrategy<SqlConnection>(connectionString, directory, @namespace);
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
