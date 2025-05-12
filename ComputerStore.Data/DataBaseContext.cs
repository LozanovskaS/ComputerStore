using ComputerStore.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStore.Data
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options)
       : base(options)
        { }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entry =>
            {
                entry.HasKey(e => e.Id);

                entry.Property(e => e.Name)
                .HasMaxLength(200)
                .IsRequired(true);

                entry.Property(e => e.Description)
               .HasMaxLength(200)
               .IsRequired(false);

          
            });

            modelBuilder.Entity<Product>(entry =>
            {
                entry.HasKey(e => e.Id);

                entry.Property(e => e.Name)
                .HasMaxLength(200)
                .IsRequired(true);

                entry.Property(e => e.Description)
               .HasMaxLength(200)
               .IsRequired(false);

                entry.Property(e => e.Price)
               .IsRequired(true);

                entry.Property(e => e.Quantity)
                .IsRequired(true);

            });

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Category)
                .WithMany(e => e.Product)
                .UsingEntity(j => j.ToTable("ProductCategory"));
           
        }
    }
}
