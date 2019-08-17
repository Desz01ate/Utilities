using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.Testing.Models;

namespace Utilities.Testing
{
    [TestFixture]
    class RepositoryServiceTest
    {
        [Test]
        public void ServiceCRUD()
        {
            var service = Service.Context;
            for (var iter = 0; iter < 10; iter++)
            {
                TestTable testTable = new TestTable() { id = iter, value = $"test" };
                var affectedCreate = service.TestTable.Insert(testTable);
                Assert.AreEqual(affectedCreate, 1);
                var selectedById = service.TestTable.Select(key: iter);
                Assert.AreEqual(selectedById.id, iter);
                Assert.AreEqual(selectedById.value, "test");
                var selectedByIdLambda = service.TestTable.Select(x => x.id == iter);
                Assert.AreEqual(selectedByIdLambda.First().id, iter);
                Assert.AreEqual(selectedByIdLambda.First().value, "test");
                var selectedAll = service.TestTable.Select();
                Assert.AreEqual(selectedAll.First().id, iter);
                Assert.AreEqual(selectedAll.First().value, "test");
                testTable.value = "updated";
                var affectedUpdate = service.TestTable.Update(testTable);
                Assert.AreEqual(affectedUpdate, 1);
                var affectedDelete = service.TestTable.Delete(testTable);
                Assert.AreEqual(affectedDelete, 1);
            }
            var affectedSelectScalar = service.TestTable.Select();
            Assert.AreEqual(affectedSelectScalar.Count(), 0);
            Assert.Pass();
        }
        [Test]
        public void ServiceCRUDWithTransaction()
        {
            var service = Service.Context;
            service.BeginTransaction();
            for (var iter = 0; iter < 10; iter++)
            {
                TestTable testTable = new TestTable() { id = iter, value = $"test" };
                var affectedCreate = service.TestTable.Insert(testTable);
                Assert.AreEqual(affectedCreate, 1);
                var selectedById = service.TestTable.Select(key: iter);
                Assert.AreEqual(selectedById.id, iter);
                Assert.AreEqual(selectedById.value, "test");
                var selectedByIdLambda = service.TestTable.Select(x => x.id == iter);
                Assert.AreEqual(selectedByIdLambda.First().id, iter);
                Assert.AreEqual(selectedByIdLambda.First().value, "test");
                testTable.value = "updated";
                var affectedUpdate = service.TestTable.Update(testTable);
                Assert.AreEqual(affectedUpdate, 1);
                //var affectedDelete = service.TestTable.Delete(testTable);
                //Assert.AreEqual(affectedDelete, 1);
            }
            service.RollbackChanges();
            var affectedSelectScalar = service.TestTable.Select();
            Assert.AreEqual(affectedSelectScalar.Count(), 0);
            Assert.Pass();
        }
    }
}
