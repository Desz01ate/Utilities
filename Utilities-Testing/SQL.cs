using MySql.Data.MySqlClient;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Testing
{
    [TestFixture]
    class SQL
    {
        private string _msSqlConnection;
        private string _mySqlConnection;
        [SetUp]
        public void Setup()
        {
            _msSqlConnection = @"Server=localhost;Database=Local;user=sa;password=sa";
            _mySqlConnection = @"Server=localhost;Database=Local;Uid=root;Pwd=root;";
        }
        [Test]
        public void SQLServer()
        {
            try
            {
                Utilities.SQL.SQLServer.ExecuteNonQuery(_msSqlConnection, $@"CREATE TABLE [dbo].[TestTable]([id] int primary key,[value] nvarchar(255))");
                for (var iter = 0; iter < 10; iter++)
                {
                    var affectedCreate = Utilities.SQL.SQLServer.ExecuteNonQuery(_msSqlConnection, @"INSERT INTO [dbo].[TestTable]([id],[value]) VALUES(@id,@value)", new[] { new SqlParameter("id", iter), new SqlParameter("value", "test") });
                    Assert.AreEqual(affectedCreate, 1);
                    var selectedById = Utilities.SQL.SQLServer.ExecuteReader<TestTable>(_msSqlConnection, @"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                    Assert.AreEqual(selectedById.First().id, iter);
                    Assert.AreEqual(selectedById.First().value, "test");
                    var selectedByIdDynamic = Utilities.SQL.SQLServer.ExecuteReader(_msSqlConnection, @"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                    Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                    Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                    var affectedUpdate = Utilities.SQL.SQLServer.ExecuteNonQuery(_msSqlConnection, @"UPDATE [dbo].[TestTable] SET [value] = @value WHERE [id] = @id", new[] { new SqlParameter("id", iter), new SqlParameter("value", "updated") });
                    Assert.AreEqual(affectedUpdate, 1);
                    var affectedDelete = Utilities.SQL.SQLServer.ExecuteNonQuery(_msSqlConnection, @"DELETE FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                    Assert.AreEqual(affectedDelete, 1);
                }
                var affectedSelectScalar = Utilities.SQL.SQLServer.ExecuteScalar<int>(_msSqlConnection, @"SELECT COUNT(1) FROM [dbo].[TestTable]");
                Assert.AreEqual(affectedSelectScalar, 0);
                Assert.Pass();
            }
            finally
            {
                Utilities.SQL.SQLServer.ExecuteNonQuery(_msSqlConnection, $@"DROP TABLE [dbo].[TestTable]");
            }
        }
        [Test]
        public async Task SQLServerAsync()
        {
            try
            {
                await Utilities.SQL.SQLServer.ExecuteNonQueryAsync(_msSqlConnection, $@"CREATE TABLE [dbo].[TestTable]([id] int primary key,[value] nvarchar(255))");
                for (var iter = 0; iter < 10; iter++)
                {
                    var affectedCreate = await Utilities.SQL.SQLServer.ExecuteNonQueryAsync(_msSqlConnection, @"INSERT INTO [dbo].[TestTable]([id],[value]) VALUES(@id,@value)", new[] { new SqlParameter("id", iter), new SqlParameter("value", "test") });
                    Assert.AreEqual(affectedCreate, 1);
                    var selectedById = await Utilities.SQL.SQLServer.ExecuteReaderAsync<TestTable>(_msSqlConnection, @"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                    Assert.AreEqual(selectedById.First().id, iter);
                    Assert.AreEqual(selectedById.First().value, "test");
                    var selectedByIdDynamic = await Utilities.SQL.SQLServer.ExecuteReaderAsync(_msSqlConnection, @"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                    Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                    Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                    var affectedUpdate = await Utilities.SQL.SQLServer.ExecuteNonQueryAsync(_msSqlConnection, @"UPDATE [dbo].[TestTable] SET [value] = @value WHERE [id] = @id", new[] { new SqlParameter("id", iter), new SqlParameter("value", "updated") });
                    Assert.AreEqual(affectedUpdate, 1);
                    var affectedDelete = await Utilities.SQL.SQLServer.ExecuteNonQueryAsync(_msSqlConnection, @"DELETE FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                    Assert.AreEqual(affectedDelete, 1);
                }
                var affectedSelectScalar = await Utilities.SQL.SQLServer.ExecuteScalarAsync<int>(_msSqlConnection, @"SELECT COUNT(1) FROM [dbo].[TestTable]");
                Assert.AreEqual(affectedSelectScalar, 0);
                Assert.Pass();
            }
            finally
            {
                await Utilities.SQL.SQLServer.ExecuteNonQueryAsync(_msSqlConnection, $@"DROP TABLE [dbo].[TestTable]");
            }
        }
        [Test]
        public void MySQL()
        {
            try
            {
                Utilities.SQL.MySQL.ExecuteNonQuery(_mySqlConnection, $@"CREATE TABLE TestTable(id int primary key,value nvarchar(255))");
                for (var iter = 0; iter < 10; iter++)
                {
                    var affectedCreate = Utilities.SQL.MySQL.ExecuteNonQuery(_mySqlConnection, @"INSERT INTO TestTable(id,value) VALUES(@id,@value)", new[] { new MySqlParameter("id", iter), new MySqlParameter("value", "test") });
                    Assert.AreEqual(affectedCreate, 1);
                    var selectedById = Utilities.SQL.MySQL.ExecuteReader<TestTable>(_mySqlConnection, @"SELECT * FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                    Assert.AreEqual(selectedById.First().id, iter);
                    Assert.AreEqual(selectedById.First().value, "test");
                    var selectedByIdDynamic = Utilities.SQL.MySQL.ExecuteReader(_mySqlConnection, @"SELECT * FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                    Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                    Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                    var affectedUpdate = Utilities.SQL.MySQL.ExecuteNonQuery(_mySqlConnection, @"UPDATE TestTable SET value = @value WHERE id = @id", new[] { new MySqlParameter("id", iter), new MySqlParameter("value", "updated") });
                    Assert.AreEqual(affectedUpdate, 1);
                    var affectedDelete = Utilities.SQL.MySQL.ExecuteNonQuery(_mySqlConnection, @"DELETE FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                    Assert.AreEqual(affectedDelete, 1);
                }
                var affectedSelectScalar = Utilities.SQL.MySQL.ExecuteScalar<Int64>(_mySqlConnection, @"SELECT COUNT(1) FROM TestTable");
                Assert.AreEqual(affectedSelectScalar, 0);
                Assert.Pass();
            }
            finally
            {
                Utilities.SQL.MySQL.ExecuteNonQuery(_mySqlConnection, $@"DROP TABLE TestTable");
            }
        }
        [Test]
        public async Task MySQLAsync()
        {
            try
            {
                await Utilities.SQL.MySQL.ExecuteNonQueryAsync(_mySqlConnection, $@"CREATE TABLE TestTable(id int primary key,value nvarchar(255))");
                for (var iter = 0; iter < 10; iter++)
                {
                    var affectedCreate = await Utilities.SQL.MySQL.ExecuteNonQueryAsync(_mySqlConnection, @"INSERT INTO TestTable(id,value) VALUES(@id,@value)", new[] { new MySqlParameter("id", iter), new MySqlParameter("value", "test") });
                    Assert.AreEqual(affectedCreate, 1);
                    var selectedById = await Utilities.SQL.MySQL.ExecuteReaderAsync<TestTable>(_mySqlConnection, @"SELECT * FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                    Assert.AreEqual(selectedById.First().id, iter);
                    Assert.AreEqual(selectedById.First().value, "test");
                    var selectedByIdDynamic = await Utilities.SQL.MySQL.ExecuteReaderAsync(_mySqlConnection, @"SELECT * FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                    Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                    Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                    var affectedUpdate = await Utilities.SQL.MySQL.ExecuteNonQueryAsync(_mySqlConnection, @"UPDATE TestTable SET value = @value WHERE id = @id", new[] { new MySqlParameter("id", iter), new MySqlParameter("value", "updated") });
                    Assert.AreEqual(affectedUpdate, 1);
                    var affectedDelete = await Utilities.SQL.MySQL.ExecuteNonQueryAsync(_mySqlConnection, @"DELETE FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                    Assert.AreEqual(affectedDelete, 1);
                }
                var affectedSelectScalar = await Utilities.SQL.MySQL.ExecuteScalarAsync<Int64>(_mySqlConnection, @"SELECT COUNT(1) FROM TestTable");
                Assert.AreEqual(affectedSelectScalar, 0);
                Assert.Pass();
            }
            finally
            {
                Utilities.SQL.MySQL.ExecuteNonQuery(_mySqlConnection, $@"DROP TABLE TestTable");
            }
        }
    }
    class TestTable
    {
        public int id { get; set; }
        public string value { get; set; }
    }
}
