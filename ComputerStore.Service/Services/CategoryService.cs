using AutoMapper;
using ComputerStore.Data.Entities;
using ComputerStore.Data.Interfaces;
using ComputerStore.Service.DTOs;
using ComputerStore.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ComputerStore.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public List<CategoryDTO> GetCategories()
        {
            var categories = _categoryRepository.GetCategories();
            return _mapper.Map<List<CategoryDTO>>(categories);
        }

        public CategoryDTO GetCategoryById(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);
            return category != null ? _mapper.Map<CategoryDTO>(category) : null;
        }

        public CategoryDTO GetCategoryByName(string name)
        {
            var category = _categoryRepository.GetCategoryByName(name);
            return category != null ? _mapper.Map<CategoryDTO>(category) : null;
        }

        public CategoryDTO AddCategory(CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            _categoryRepository.AddCategory(category);
            return _mapper.Map<CategoryDTO>(category);
        }

        public CategoryDTO UpdateCategory(CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            var updatedCategory = _categoryRepository.UpdateCategory(category);
            return updatedCategory != null ? _mapper.Map<CategoryDTO>(updatedCategory) : null;
        }

        public bool DeleteCategory(int id)
        {
            return _categoryRepository.DeleteCategory(id);
        }
    }
}