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
        private string _sqlConnection;
        [SetUp]
        public void Setup()
        {
            _sqlConnection = @"Server=localhost;Database=Local;user=sa;password=sa";
        }
        [Test]
        public void SQLServer()
        {
            Utilities.SQL.SQLServer.ExecuteNonQuery(_sqlConnection, $@"CREATE TABLE [dbo].[TestTable]([id] int primary key,[value] nvarchar(255))");
            for (var iter = 0; iter < 10; iter++)
            {
                var affectedCreate = Utilities.SQL.SQLServer.ExecuteNonQuery(_sqlConnection, @"INSERT INTO [dbo].[TestTable]([id],[value]) VALUES(@id,@value)", new[] { new SqlParameter("id", iter), new SqlParameter("value", "test") });
                Assert.AreEqual(affectedCreate, 1);
                var selectedById = Utilities.SQL.SQLServer.ExecuteReader<TestTable>(_sqlConnection, @"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                Assert.AreEqual(selectedById.First().id, iter);
                Assert.AreEqual(selectedById.First().value, "test");
                var selectedByIdDynamic = Utilities.SQL.SQLServer.ExecuteReader(_sqlConnection, @"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                Assert.AreEqual(selectedByIdDynamic.First().id, iter.ToString());
                Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                var affectedUpdate = Utilities.SQL.SQLServer.ExecuteNonQuery(_sqlConnection, @"UPDATE [dbo].[TestTable] SET [value] = @value WHERE [id] = @id", new[] { new SqlParameter("id", iter), new SqlParameter("value", "updated") });
                Assert.AreEqual(affectedUpdate, 1);
                var affectedDelete = Utilities.SQL.SQLServer.ExecuteNonQuery(_sqlConnection, @"DELETE FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                Assert.AreEqual(affectedDelete, 1);
            }
            var affectedSelectScalar = Utilities.SQL.SQLServer.ExecuteScalar<int>(_sqlConnection, @"SELECT COUNT(1) FROM [dbo].[TestTable]");
            Assert.AreEqual(affectedSelectScalar, 0);
            Utilities.SQL.SQLServer.ExecuteNonQuery(_sqlConnection, $@"DROP TABLE [dbo].[TestTable]");
            Assert.Pass();
        }
        [Test]
        public async Task SQLServerAsync()
        {
            await Utilities.SQL.SQLServer.ExecuteNonQueryAsync(_sqlConnection, $@"CREATE TABLE [dbo].[TestTable]([id] int primary key,[value] nvarchar(255))");
            for (var iter = 0; iter < 10; iter++)
            {
                var affectedCreate = await Utilities.SQL.SQLServer.ExecuteNonQueryAsync(_sqlConnection, @"INSERT INTO [dbo].[TestTable]([id],[value]) VALUES(@id,@value)", new[] { new SqlParameter("id", iter), new SqlParameter("value", "test") });
                Assert.AreEqual(affectedCreate, 1);
                var selectedById = await Utilities.SQL.SQLServer.ExecuteReaderAsync<TestTable>(_sqlConnection, @"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                Assert.AreEqual(selectedById.First().id, iter);
                Assert.AreEqual(selectedById.First().value, "test");
                var selectedByIdDynamic = await Utilities.SQL.SQLServer.ExecuteReaderAsync(_sqlConnection, @"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                Assert.AreEqual(selectedByIdDynamic.First().id, iter.ToString());
                Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                var affectedUpdate = await Utilities.SQL.SQLServer.ExecuteNonQueryAsync(_sqlConnection, @"UPDATE [dbo].[TestTable] SET [value] = @value WHERE [id] = @id", new[] { new SqlParameter("id", iter), new SqlParameter("value", "updated") });
                Assert.AreEqual(affectedUpdate, 1);
                var affectedDelete = await Utilities.SQL.SQLServer.ExecuteNonQueryAsync(_sqlConnection, @"DELETE FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                Assert.AreEqual(affectedDelete, 1);
            }
            var affectedSelectScalar = await Utilities.SQL.SQLServer.ExecuteScalarAsync<int>(_sqlConnection, @"SELECT COUNT(1) FROM [dbo].[TestTable]");
            Assert.AreEqual(affectedSelectScalar, 0);
            await Utilities.SQL.SQLServer.ExecuteNonQueryAsync(_sqlConnection, $@"DROP TABLE [dbo].[TestTable]");
            Assert.Pass();
        }
    }
    class TestTable
    {
        public int id { get; set; }
        public string value { get; set; }
    }
}
