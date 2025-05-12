using ComputerStore.Data.Entities;
using ComputerStore.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputerStore.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataBaseContext _context;

        public ProductRepository(DataBaseContext context)
        {
            _context = context;
        }

        public List<Product> GetProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);
        }

        public Product GetProductByName(string name)
        {
            return _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Name == name);
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            var existingProduct = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == product.Id);

            if (existingProduct == null)
                return;

            // Update scalar properties
            _context.Entry(existingProduct).CurrentValues.SetValues(product);

            // Handle categories - clear and add
            existingProduct.Category.Clear();
            foreach (var category in product.Category)
            {
                var existingCategory = _context.Categories.Find(category.Id);
                if (existingCategory != null)
                {
                    existingProduct.Category.Add(existingCategory);
                }
                else
                {
                    existingProduct.Category.Add(category);
                }
            }

            _context.SaveChanges();
        }

        public bool DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateStock(int productId, int quantity)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return false;
            }

            product.Quantity = quantity;
            _context.SaveChanges();
            return true;
        }

        public List<string> GetProductCategories(int productId)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == productId);

            return product?.Category.Select(c => c.Name).ToList() ?? new List<string>();
        }
    }
}