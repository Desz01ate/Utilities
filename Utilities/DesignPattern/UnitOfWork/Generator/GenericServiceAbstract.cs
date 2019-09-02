using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using Utilities.Interfaces;
using Utilities.SQL.Generator;
using Utilities.SQL.Generator.Model;

namespace Utilities.DesignPattern.UnitOfWork
{
    /// <summary>
    /// Abstract class which defined how the generator should work.
    /// </summary>
    /// <typeparam name="TDatabase"></typeparam>
    public abstract class GenericServiceAbstract<TDatabase> : IGeneratorStrategy
        where TDatabase : DbConnection, new()
    {
        protected CSharpGenerator<TDatabase> generator;

        public string Directory { get; protected set; }

        public string Namespace { get; protected set; }

        public string ConnectionString { get; protected set; }
        public string ModelDirectory => Path.Combine(Directory, "Models");
        public string RepositoryDirectory => Path.Combine(Directory, "Repositories");
        public IEnumerable<string> Table { get; private set; }
        private void GenerateRepositories(string targetNamespace)
        {
            //var files = Directory.EnumerateFiles(modelDirectory);
            foreach (var table in Table)
            {
                generator.GenerateFromSpecificTable(table, GenerateRepository);
            }
        }
        protected string TableNameCleanser(string tableName)
        {
            return tableName.Replace("-", "");
        }
        protected virtual void GenerateModel()
        {
            generator.GenerateAllTable();
        }
        protected abstract void GenerateRepository(Table tb);
        protected abstract void GenerateService();
        public virtual void Generate()
        {
            System.IO.Directory.CreateDirectory(ModelDirectory);
            System.IO.Directory.CreateDirectory(RepositoryDirectory);
            this.generator = new CSharpGenerator<TDatabase>(ConnectionString, ModelDirectory, $"{Namespace}.Models");
            Table = generator.Tables;
            generator.GenerateAllTable();
            GenerateRepositories(Namespace);
            GenerateService();
        }
    }
}
