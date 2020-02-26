using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;

namespace EFDemoApp.Tests
{
    public class TestDbContextHelper
    {
        public TestDbContext CreateSqlLiteInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            var context = new TestDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();
            context.TestCategories.AddRange(TestCategories());
            context.TestProducts.AddRange(TestProducts());
            context.SaveChanges();
            return context;
        }

        private List<TestCategory> TestCategories()
        {
            return Builder<TestCategory>.CreateListOfSize(20).Build().ToList();
        }

        private List<TestProduct> TestProducts()
        {
            var productList = Builder<TestProduct>.CreateListOfSize(20)
                .TheFirst(5)
                .With(x => x.CategoryId = 1)
                .With(x => x.InStock = true)
                .TheNext(5)
                .With(x => x.InStock = false)
                .With(y => y.Stock = 0)
                .Build();

            return productList.ToList();
        }

        public static TestDbContextHelper Instance = new TestDbContextHelper();
    }
}