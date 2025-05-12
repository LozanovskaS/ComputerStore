
using ComputerStore.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ComputerStore.Data;
using ComputerStore.Data.Interfaces;
using ComputerStore.Data.Repositories;
using ComputerStore.Service.Interfaces;
using ComputerStore.Service.Profiles;
using ComputerStore.Service.Services;
using ComputerStore.WebApi.Infrastructure;

namespace UniversityApplication.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Database

            builder.Services.Configure<ConnectionStrings>(
                builder.Configuration.GetSection("ConnectionStrings"));

            // DB context
            builder.Services.AddDbContext<DataBaseContext>(options =>
            {
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("PostgreConnection"),
                    npgsql => npgsql.MigrationsAssembly("ComputerStore.Data"));

            });

            // AutoMapper
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<CategoryMappingProfile>();
                cfg.AddProfile<ProductMappingProfile>();
            });


            // Register repositories
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            // Register services (you probably already have these)
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
