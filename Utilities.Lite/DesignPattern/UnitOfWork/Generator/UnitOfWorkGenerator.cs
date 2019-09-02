using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Utilities.Interfaces;
using Utilities.SQL.Generator;

namespace Utilities.DesignPattern.UnitOfWork.Generator
{
    public class UnitOfWorkGenerator<TDatabase> where TDatabase : DbConnection, new()
    {
        IGeneratorStrategy<TDatabase> _strategy;
        public void SetStrategy(IGeneratorStrategy<TDatabase> strategy)
        {
            _strategy = strategy;
        }
        public void Generate()
        {
            if (_strategy == null) throw new NullReferenceException("Strategy must not be null.");
            System.IO.Directory.CreateDirectory(_strategy.ModelDirectory);
            System.IO.Directory.CreateDirectory(_strategy.RepositoryDirectory);
            GenerateModel();
            GenerateRepositories();
            GenerateService();
        }
        private void GenerateModel()
        {
            _strategy.GenerateModel();
        }
        private void GenerateRepositories()
        {
            foreach (var table in _strategy.Generator.Tables)
            {
                _strategy.Generator.GenerateFromSpecificTable(table, _strategy.GenerateRepository);
            }
        }
        private void GenerateService()
        {
            _strategy.GenerateService();
        }
    }
}
