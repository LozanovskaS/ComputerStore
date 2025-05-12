using ComputerStore.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStore.Service.Interfaces
{
    public interface ICategoryService
    {
        List<CategoryDTO> GetCategories();
        CategoryDTO GetCategoryById(int id);
        CategoryDTO GetCategoryByName(string name);
        CategoryDTO AddCategory(CategoryDTO category);
        CategoryDTO UpdateCategory(CategoryDTO category);
        bool DeleteCategory(int id);
    }
}
