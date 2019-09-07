using ModelGenerator.Services.Generator;
using ModelGenerator.Services.Generator.Model;
using System.Data.Common;

namespace ModelGenerator.Services.DesignPattern.Interfaces
{
    /// <summary>
    /// Provide neccessay contact for code generator factory.
    /// </summary>
    /// <typeparam name="TDatabase"></typeparam>
    public interface IGeneratorStrategy<TDatabase> where TDatabase : DbConnection, new()
    {
        /// <summary>
        /// Target base directory of all files (model,repository and service).
        /// </summary>
        string Directory { get; }
        /// <summary>
        /// Target namespace.
        /// </summary>
        string Namespace { get; }
        /// <summary>
        /// Connection string of database.
        /// </summary>
        string ConnectionString { get; }
        /// <summary>
        /// Target directory for model files.
        /// </summary>
        string ModelDirectory { get; }
        /// <summary>
        /// Target directory for repository files.
        /// </summary>
        string RepositoryDirectory { get; }
        /// <summary>
        /// Internal language-specific generator.
        /// </summary>
        AbstractModelGenerator<TDatabase> Generator { get; }
        /// <summary>
        /// Method for model generate.
        /// </summary>
        void GenerateModel();
        /// <summary>
        /// Method for repository generate.
        /// </summary>
        /// <param name="table"></param>
        void GenerateRepository(Table table);
        /// <summary>
        /// Method for service generate.
        /// </summary>
        void GenerateService();
    }
}
