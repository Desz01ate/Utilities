using ModelGenerator.Services.DesignPattern.Interfaces;
using System;
using System.Data.Common;

namespace ModelGenerator.Services.DesignPattern.UnitOfWork.Generator
{
    public class UnitOfWorkGenerator<TDatabase> where TDatabase : DbConnection, new()
    {
        IGeneratorStrategy<TDatabase> _strategy;
        public void UseStrategy(IGeneratorStrategy<TDatabase> strategy)
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
