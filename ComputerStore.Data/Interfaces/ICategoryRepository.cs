using ComputerStore.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStore.Data.Interfaces
{
    public interface ICategoryRepository
    {
        List<Category> GetCategories();
        Category GetCategoryById(int id);
        Category GetCategoryByName(string name);
        void AddCategory(Category category);
        Category UpdateCategory(Category category);
        bool DeleteCategory(int id);

    }
}
