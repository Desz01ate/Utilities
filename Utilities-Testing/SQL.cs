using MySql.Data.MySqlClient;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Attributes.SQL;
using Utilities.SQL;

namespace Utilities.Testing
{
    [TestFixture]
    class SQL
    {
        private string _msSqlConnection;
        private string _mySqlConnection;
        private string _sqliteConnection;
        [SetUp]
        public void Setup()
        {
            _msSqlConnection = @"Server=localhost;Database=Local;user=sa;password=sa;";
            _mySqlConnection = @"Server=localhost;Database=Local;Uid=root;Pwd=;";
            _sqliteConnection = @"Data Source=C:\Users\TYCHE\Documents\SQLite\Local.db;Version=3;";
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
        public void SQLServerCRUD()
        {
            try
            {
                Utilities.SQL.SQLServer.ExecuteNonQuery(_msSqlConnection, $@"CREATE TABLE [dbo].[TestTable]([id] int primary key,[value] nvarchar(255))");
                for (var iter = 0; iter < 10; iter++)
                {
                    var affectedCreate = Utilities.SQL.SQLServer.Insert(_msSqlConnection, new TestTable()
                    {
                        id = iter,
                        value = "test"
                    });//Utilities.SQL.SQLServer.ExecuteNonQuery(_msSqlConnection, @"INSERT INTO [dbo].[TestTable]([id],[value]) VALUES(@id,@value)", new[] { new SqlParameter("id", iter), new SqlParameter("value", "test") });
                    Assert.AreEqual(affectedCreate, 1);
                    var selectedById = Utilities.SQL.SQLServer.Select<TestTable>(_msSqlConnection, iter);
                    Assert.AreEqual(selectedById.id, iter);
                    Assert.AreEqual(selectedById.value, "test");
                    var selectedByIdDynamic = Utilities.SQL.SQLServer.ExecuteReader(_msSqlConnection, @"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                    Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                    Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                    selectedById.value = "updated";
                    var affectedUpdate = Utilities.SQL.SQLServer.Update(_msSqlConnection, selectedById);
                    Assert.AreEqual(affectedUpdate, 1);
                    var affectedDelete = Utilities.SQL.SQLServer.Delete(_msSqlConnection, selectedById);
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
        public void SQLServerConnector()
        {
            using (var connection = new SQLServer(_msSqlConnection))
            {
                try
                {
                    connection.ExecuteNonQuery($@"CREATE TABLE [dbo].[TestTable]([id] int primary key,[value] nvarchar(255))");
                    for (var iter = 0; iter < 10; iter++)
                    {
                        var affectedCreate = connection.ExecuteNonQuery(@"INSERT INTO [dbo].[TestTable]([id],[value]) VALUES(@id,@value)", new[] { new SqlParameter("id", iter), new SqlParameter("value", "test") });
                        Assert.AreEqual(affectedCreate, 1);
                        var selectedById = connection.ExecuteReader<TestTable>(@"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                        Assert.AreEqual(selectedById.First().id, iter);
                        Assert.AreEqual(selectedById.First().value, "test");
                        var selectedByIdDynamic = connection.ExecuteReader(@"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                        Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                        Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                        var affectedUpdate = connection.ExecuteNonQuery(@"UPDATE [dbo].[TestTable] SET [value] = @value WHERE [id] = @id", new[] { new SqlParameter("id", iter), new SqlParameter("value", "updated") });
                        Assert.AreEqual(affectedUpdate, 1);
                        var affectedDelete = connection.ExecuteNonQuery(@"DELETE FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                        Assert.AreEqual(affectedDelete, 1);
                    }
                    var affectedSelectScalar = connection.ExecuteScalar<int>(@"SELECT COUNT(1) FROM [dbo].[TestTable]");
                    Assert.AreEqual(affectedSelectScalar, 0);
                    Assert.Pass();
                }
                finally
                {
                    connection.ExecuteNonQuery($@"DROP TABLE [dbo].[TestTable]");
                }
            }
        }
        [Test]
        public async Task SQLServerConnectorAsync()
        {
            using (var connection = new SQLServer(_msSqlConnection))
            {
                try
                {
                    await connection.ExecuteNonQueryAsync($@"CREATE TABLE [dbo].[TestTable]([id] int primary key,[value] nvarchar(255))");
                    for (var iter = 0; iter < 10; iter++)
                    {
                        var affectedCreate = await connection.ExecuteNonQueryAsync(@"INSERT INTO [dbo].[TestTable]([id],[value]) VALUES(@id,@value)", new[] { new SqlParameter("id", iter), new SqlParameter("value", "test") });
                        Assert.AreEqual(affectedCreate, 1);
                        var selectedById = await connection.ExecuteReaderAsync<TestTable>(@"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                        Assert.AreEqual(selectedById.First().id, iter);
                        Assert.AreEqual(selectedById.First().value, "test");
                        var selectedByIdDynamic = await connection.ExecuteReaderAsync(@"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                        Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                        Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                        var affectedUpdate = await connection.ExecuteNonQueryAsync(@"UPDATE [dbo].[TestTable] SET [value] = @value WHERE [id] = @id", new[] { new SqlParameter("id", iter), new SqlParameter("value", "updated") });
                        Assert.AreEqual(affectedUpdate, 1);
                        var affectedDelete = await connection.ExecuteNonQueryAsync(@"DELETE FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                        Assert.AreEqual(affectedDelete, 1);
                    }
                    var affectedSelectScalar = await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(1) FROM [dbo].[TestTable]");
                    Assert.AreEqual(affectedSelectScalar, 0);
                    Assert.Pass();
                }
                finally
                {
                    await connection.ExecuteNonQueryAsync($@"DROP TABLE [dbo].[TestTable]");
                }
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
        public void MySQLCRUD()
        {
            try
            {
                Utilities.SQL.MySQL.ExecuteNonQuery(_mySqlConnection, $@"CREATE TABLE TestTable(id int primary key,value nvarchar(255))");
                for (var iter = 0; iter < 10; iter++)
                {
                    var affectedCreate = Utilities.SQL.MySQL.Insert(_mySqlConnection, new TestTable()
                    {
                        id = iter,
                        value = "test"
                    });//Utilities.SQL.SQLServer.ExecuteNonQuery(_msSqlConnection, @"INSERT INTO [dbo].[TestTable]([id],[value]) VALUES(@id,@value)", new[] { new SqlParameter("id", iter), new SqlParameter("value", "test") });
                    Assert.AreEqual(affectedCreate, 1);
                    var selectedById = Utilities.SQL.MySQL.Select<TestTable>(_mySqlConnection, iter);
                    Assert.AreEqual(selectedById.id, iter);
                    Assert.AreEqual(selectedById.value, "test");
                    var selectedByIdDynamic = Utilities.SQL.MySQL.ExecuteReader(_mySqlConnection, @"SELECT * FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                    Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                    Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                    selectedById.value = "updated";
                    var affectedUpdate = Utilities.SQL.MySQL.Update(_mySqlConnection, selectedById);
                    Assert.AreEqual(affectedUpdate, 1);
                    var affectedDelete = Utilities.SQL.MySQL.Delete(_mySqlConnection, selectedById);
                    Assert.AreEqual(affectedDelete, 1);
                }
                var affectedSelectScalar = Utilities.SQL.MySQL.ExecuteScalar<long>(_mySqlConnection, @"SELECT COUNT(1) FROM TestTable");
                Assert.AreEqual(affectedSelectScalar, 0);
                Assert.Pass();
            }
            catch (Exception e)
            {

            }
            finally
            {
                Utilities.SQL.MySQL.ExecuteNonQuery(_mySqlConnection, $@"DROP TABLE TestTable");
            }
        }
        [Test]
        public void MySQLConnector()
        {
            using (var connection = new MySQL(_mySqlConnection))
            {
                try
                {
                    connection.ExecuteNonQuery($@"CREATE TABLE TestTable(id int primary key,value nvarchar(255))");
                    for (var iter = 0; iter < 10; iter++)
                    {
                        var affectedCreate = connection.ExecuteNonQuery(@"INSERT INTO TestTable(id,value) VALUES(@id,@value)", new[] { new MySqlParameter("id", iter), new MySqlParameter("value", "test") });
                        Assert.AreEqual(affectedCreate, 1);
                        var selectedById = connection.ExecuteReader<TestTable>(@"SELECT * FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                        Assert.AreEqual(selectedById.First().id, iter);
                        Assert.AreEqual(selectedById.First().value, "test");
                        var selectedByIdDynamic = connection.ExecuteReader(@"SELECT * FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                        Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                        Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                        var affectedUpdate = connection.ExecuteNonQuery(@"UPDATE TestTable SET value = @value WHERE id = @id", new[] { new MySqlParameter("id", iter), new MySqlParameter("value", "updated") });
                        Assert.AreEqual(affectedUpdate, 1);
                        var affectedDelete = connection.ExecuteNonQuery(@"DELETE FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                        Assert.AreEqual(affectedDelete, 1);
                    }
                    var affectedSelectScalar = connection.ExecuteScalar<long>(@"SELECT COUNT(1) FROM TestTable");
                    Assert.AreEqual(affectedSelectScalar, 0);
                    Assert.Pass();
                }
                finally
                {
                    connection.ExecuteNonQuery($@"DROP TABLE TestTable");
                }
            }
        }
        [Test]
        public async Task MySQLConnectorAsync()
        {
            using (var connection = new MySQL(_mySqlConnection))
            {
                try
                {
                    await connection.ExecuteNonQueryAsync($@"CREATE TABLE TestTable(id int primary key,value nvarchar(255))");
                    for (var iter = 0; iter < 10; iter++)
                    {
                        var affectedCreate = await connection.ExecuteNonQueryAsync(@"INSERT INTO TestTable(id,value) VALUES(@id,@value)", new[] { new MySqlParameter("id", iter), new MySqlParameter("value", "test") });
                        Assert.AreEqual(affectedCreate, 1);
                        var selectedById = await connection.ExecuteReaderAsync<TestTable>(@"SELECT * FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                        Assert.AreEqual(selectedById.First().id, iter);
                        Assert.AreEqual(selectedById.First().value, "test");
                        var selectedByIdDynamic = await connection.ExecuteReaderAsync(@"SELECT * FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                        Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                        Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                        var affectedUpdate = await connection.ExecuteNonQueryAsync(@"UPDATE TestTable SET value = @value WHERE id = @id", new[] { new MySqlParameter("id", iter), new MySqlParameter("value", "updated") });
                        Assert.AreEqual(affectedUpdate, 1);
                        var affectedDelete = await connection.ExecuteNonQueryAsync(@"DELETE FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                        Assert.AreEqual(affectedDelete, 1);
                    }
                    var affectedSelectScalar = await connection.ExecuteScalarAsync<long>(@"SELECT COUNT(1) FROM TestTable");
                    Assert.AreEqual(affectedSelectScalar, 0);
                    Assert.Pass();
                }
                finally
                {
                    await connection.ExecuteNonQueryAsync($@"DROP TABLE TestTable");
                }
            }
        }
        [Test]
        public void SQLiteConnector()
        {
            using (var connection = new SQLite(_sqliteConnection))
            {
                try
                {
                    connection.ExecuteNonQuery($@"CREATE TABLE TestTable(id int primary key,value nvarchar(255))");
                    for (var iter = 0; iter < 10; iter++)
                    {
                        var affectedCreate = connection.ExecuteNonQuery(@"INSERT INTO TestTable(id,value) VALUES(@id,@value)", new[] { new SQLiteParameter("id", iter), new SQLiteParameter("value", "test") });
                        Assert.AreEqual(affectedCreate, 1);
                        var selectedById = connection.ExecuteReader<TestTable>(@"SELECT * FROM TestTable WHERE id = @id", new[] { new SQLiteParameter("id", iter) });
                        Assert.AreEqual(selectedById.First().id, iter);
                        Assert.AreEqual(selectedById.First().value, "test");
                        var selectedByIdDynamic = connection.ExecuteReader(@"SELECT * FROM TestTable WHERE id = @id", new[] { new SQLiteParameter("id", iter) });
                        Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                        Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                        var affectedUpdate = connection.ExecuteNonQuery(@"UPDATE TestTable SET value = @value WHERE id = @id", new[] { new SQLiteParameter("id", iter), new SQLiteParameter("value", "updated") });
                        Assert.AreEqual(affectedUpdate, 1);
                        var affectedDelete = connection.ExecuteNonQuery(@"DELETE FROM TestTable WHERE id = @id", new[] { new SQLiteParameter("id", iter) });
                        Assert.AreEqual(affectedDelete, 1);
                    }
                    var affectedSelectScalar = connection.ExecuteScalar<long>(@"SELECT COUNT(1) FROM TestTable");
                    Assert.AreEqual(affectedSelectScalar, 0);
                    Assert.Pass();
                }
                finally
                {
                    connection.ExecuteNonQuery($@"DROP TABLE TestTable");
                }
            }
        }
    }
    class TestTable
    {
        [PrimaryKey]
        public int id { get; set; }
        public string value { get; set; }
    }
}
