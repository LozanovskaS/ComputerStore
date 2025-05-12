using ComputerStore.Data;
using ComputerStore.Data.Entities;
using ComputerStore.Data.Interfaces;
using ComputerStore.Data.Repositories;
using ComputerStore.Service.Interfaces;
using ComputerStore.Service.Profiles;
using ComputerStore.Service.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ComputerStore.IntegrationTests.Base
{
    public abstract class IntegrationTestBase : IDisposable
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly DataBaseContext DbContext;

        protected IntegrationTestBase()
        {
            var services = new ServiceCollection();

            services.AddDbContext<DataBaseContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

            services.AddAutoMapper(cfg => {
                cfg.AddProfile<CategoryMappingProfile>();
                cfg.AddProfile<ProductMappingProfile>();
            });

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();

            ServiceProvider = services.BuildServiceProvider();
            DbContext = ServiceProvider.GetRequiredService<DataBaseContext>();

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var cpuCategory = new Category
            {
                Id = 1,
                Name = "CPU",
                Description = "Central Processing Units"
            };

            var storageCategory = new Category
            {
                Id = 2,
                Name = "Storage",
                Description = "Storage devices"
            };

            DbContext.Categories.Add(cpuCategory);
            DbContext.Categories.Add(storageCategory);

            var product1 = new Product
            {
                Id = 1,
                Name = "Intel Core i9-9900K",
                Description = "High-performance desktop processor",
                Price = 475.99m,
                Quantity = 10,
                Category = new List<Category> { cpuCategory }
            };

            var product2 = new Product
            {
                Id = 2,
                Name = "Samsung 970 EVO",
                Description = "NVMe SSD",
                Price = 149.99m,
                Quantity = 15,
                Category = new List<Category> { storageCategory }
            };

            DbContext.Products.Add(product1);
            DbContext.Products.Add(product2);

            DbContext.SaveChanges();
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Dispose();
        }
    }
}