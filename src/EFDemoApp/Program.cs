using System;
using System.Linq;
using Common.UoW.EF;
using EFDemoApp.Tests;
using Microsoft.EntityFrameworkCore;

namespace EFDemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var testDbContextHelper = TestDbContextHelper.Instance;
            using (var dbContext = testDbContextHelper.CreateSqlLiteInMemoryContext())
            {
                using (var uow = new UnitOfWork<TestDbContext>(dbContext))
                {
                    var categoryRepos = uow.GetRepository<TestCategory>();
                    var testCategories = categoryRepos.Query().AsNoTracking().ToList();
                    foreach (var testCategory in testCategories)
                    {
                        Console.WriteLine("{0},{1}", testCategory.Id, testCategory.Name);
                    }

                    Console.WriteLine("------------");

                    var productRepos = uow.GetRepository<TestProduct>();
                    var testProducts = productRepos.Query().Include(x => x.Category).AsNoTracking().ToList();
                    foreach (var testProduct in testProducts)
                    {
                        Console.WriteLine("{0},{1}, {2}", testProduct.Id, testProduct.Name, testProduct.Category.Name);
                    }
                }
            }
            Console.WriteLine("----------------");
            Console.Read();
        }
    }
}
