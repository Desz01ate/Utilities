//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.IO;
//using System.Text;
//using Utilities.Interfaces;
//using Utilities.SQL.Generator;
//using Utilities.SQL.Generator.Model;

//namespace Utilities.DesignPattern.Repository
//{
//    public abstract class GenericServiceAbstract<TDatabase> : IGeneratorStrategy
//        where TDatabase : DbConnection, new()
//    {
//        protected ModelGenerator<TDatabase> generator;

//        public string Directory { get; protected set; }

//        public string Namespace { get; protected set; }

//        public string ConnectionString { get; protected set; }
//        public string ModelDirectory => Path.Combine(Directory, "Models");
//        public string RepositoryDirectory => Path.Combine(Directory, "Repositories");

//        protected virtual void GenerateRepositories(string targetNamespace)
//        {
//            //var files = Directory.EnumerateFiles(modelDirectory);
//            foreach (var table in generator.Tables)
//            {
//                generator.GenerateFromTable(table, GenerateRepository);
//            }
//        }
//        protected string TableNameCleanser(string tableName)
//        {
//            return tableName.Replace("-", "");
//        }
//        protected virtual void GenerateRepository(Table tb)
//        {
//            throw new NotImplementedException();
//        }
//        protected virtual void GenerateService()
//        {
//            throw new NotImplementedException();
//        }
//        public virtual void Generate()
//        {
//            System.IO.Directory.CreateDirectory(ModelDirectory);
//            System.IO.Directory.CreateDirectory(RepositoryDirectory);
//            this.generator = new ModelGenerator<TDatabase>(ConnectionString, ModelDirectory, $"{Namespace}.Models");
//            generator.GenerateAllTables(SQL.Generator.Enumerable.TargetLanguage.CSharp);
//            GenerateRepositories(Namespace);
//            GenerateService();
//        }
//    }
//}
