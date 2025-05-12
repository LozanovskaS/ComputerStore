using ComputerStore.Data.Entities;
using ComputerStore.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputerStore.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataBaseContext _context;

        public CategoryRepository(DataBaseContext context)
        {
            _context = context;
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategoryById(int id)
        {
            return _context.Categories.Find(id);
        }

        public Category GetCategoryByName(string name)
        {
            return _context.Categories
                .FirstOrDefault(c => c.Name.ToLower() == name.ToLower());
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public Category UpdateCategory(Category category)
        {
            var existing = _context.Categories.Find(category.Id);
            if (existing == null) return null;

            // Update properties
            _context.Entry(existing).CurrentValues.SetValues(category);
            _context.SaveChanges();

            return existing;
        }

        public bool DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return true;
        }
    }
}