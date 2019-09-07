using CommandLine;
using ModelGenerator.Services.DesignPattern.Interfaces;
using ModelGenerator.Services.DesignPattern.UnitOfWork.Generator;
using ModelGenerator.Services.DesignPattern.UnitOfWork.Strategy.NonSingleton;
using ModelGenerator.Services.Generator;
using ModelGenerator.Services.Generator.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ModelGenerator
{
    public class Options
    {
        [Option('c', "connection", Required = true, HelpText = "Set connection string to generator.")]
        public string ConnectionString { get; set; }
        [Option('o', "output", Required = true, HelpText = "Set output directory.")]
        public string Directory { get; set; }
        [Option('l', "language", Required = true, HelpText = "Set target language for generator, which currently support [C#,Java,PHP,Python,Python37,TypeScript,VisualBasic]")]
        public string Language { get; set; }
        [Option('n', "namespace", Required = false, HelpText = "Set the model namespace (optional).")]
        public string Namespace { get; set; }
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages (optional).")]
        public bool Verbose { get; set; }
        [Option('t', "type", Required = true, HelpText = "Set the generate output type (model,unitofwork).")]
        public string Type { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, Action<Options>> AvailableType = new Dictionary<string, Action<Options>>()
            {
                { "model", GenerateModel },
                { "unitofwork" , GenerateUnitOfWork }
            };
            Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
            {
                if (!AvailableType.TryGetValue(options.Type, out var action))
                {
                    Console.WriteLine("Input type not found.");
                }
                action(options);
            }).WithNotParsed<Options>(err =>
            {
                Console.Write("Connection string : ");
                var conStr = Console.ReadLine();
                Console.Write("Output directory : ");
                var outputDir = Console.ReadLine();
                Console.Write("Target language : ");
                var lang = Console.ReadLine();
                Console.Write("Target output type : ");
                var type = Console.ReadLine();
                Console.Write("Namespace (optional) : ");
                var @namespace = Console.ReadLine();

                var options = new Options()
                {
                    ConnectionString = conStr,
                    Directory = outputDir,
                    Language = lang,
                    Namespace = @namespace,
                    Type = type
                };
                if (!AvailableType.TryGetValue(options.Type, out var action))
                {
                    Console.WriteLine("Input type not found.");
                }
                action(options);
            });
        }

        private static void GenerateModel(Options options)
        {
            var dotnetLang = new[] { "c#", "vb", "visualbasic" };
            var supportedLanguage = new Dictionary<string, Func<string, string, string, IModelGenerator>> {
                { "c#", (c,o,n) => new CSharpGenerator<SqlConnection>(c,o,n) },
                { "java", (c,o,n) => new JavaGenerator<SqlConnection>(c,o,n) },
                { "php" , (c,o,n)=> new PHPGenerator<SqlConnection>(c,o,n) },
                { "python", (c,o,n) => new PythonGenerator<SqlConnection>(c,o,n) },
                { "python37", (c,o,n) => new Python37Generator<SqlConnection>(c,o,n) },
                { "typescript", (c,o,n) => new TypeScriptGenerator<SqlConnection>(c,o,n) } ,
                { "ts", (c,o,n) => new TypeScriptGenerator<SqlConnection>(c,o,n) } ,
                { "visualbasic", (c,o,n) => new VisualBasicGenerator<SqlConnection>(c,o,n) },
                { "vb", (c,o,n) => new VisualBasicGenerator<SqlConnection>(c,o,n) }
            };

            var targetLanguage = options.Language?.ToLower();
            if (!supportedLanguage.TryGetValue(targetLanguage, out var generatorInjector))
            {
                throw new Exception($"Not supported language {targetLanguage} (currently support {string.Join(",", supportedLanguage.Select(x => x.Key))})");
            }
            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new ArgumentNullException("Connection string must not be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(options.Directory))
            {
                throw new ArgumentNullException("Directory must not be null or empty.");
            }
            var generator = generatorInjector(options.ConnectionString, options.Directory, options.Namespace);
            generator.GenerateAllTable();
        }
        private static void GenerateUnitOfWork(Options options)
        {
            var supportedLanguage = new Dictionary<string, Func<string, string, string, IGeneratorStrategy<SqlConnection>>> {
                { "c#", (c,o,n) => new CSharpSingletonStrategy<SqlConnection>(c,o,n) },
                { "csharp",(c,o,n) => new CSharpSingletonStrategy<SqlConnection>(c,o,n) },
                { "visualbasic", (c,o,n) => new VisualBasicSingletonStrategy<SqlConnection>(c,o,n) },
                { "vb", (c,o,n) => new VisualBasicSingletonStrategy<SqlConnection>(c,o,n) }
            };

            var targetLanguage = options.Language?.ToLower();
            if (!supportedLanguage.TryGetValue(targetLanguage, out var strategy))
            {
                throw new Exception($"Not supported language {targetLanguage} (currently support {string.Join(",", supportedLanguage.Select(x => x.Key))})");
            }
            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new ArgumentNullException("Connection string must not be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(options.Directory))
            {
                throw new ArgumentNullException("Directory must not be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(options.Namespace))
            {
                options.Namespace = "MyNamespace";
            }
            //var generator = generatorInjector(options.ConnectionString, options.Directory, options.Namespace);
            //generator.GenerateModel();
            var generator = new UnitOfWorkGenerator<SqlConnection>();
            generator.UseStrategy(strategy(options.ConnectionString, options.Directory, options.Namespace));
            generator.Generate();
        }

    }
}
