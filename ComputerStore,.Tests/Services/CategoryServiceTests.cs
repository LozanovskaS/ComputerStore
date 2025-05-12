using AutoMapper;
using ComputerStore.Data.Entities;
using ComputerStore.Data.Interfaces;
using ComputerStore.Service.DTOs;
using ComputerStore.Service.Services;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ComputerStore.Tests.Services
{
    public class CategoryServiceTests
    {
        ICategoryRepository categoryRepo;
        IMapper mapper;
        Mock<ICategoryRepository> categoryRepositoryMock = new Mock<ICategoryRepository>();
        Mock<IMapper> mapperMock = new Mock<IMapper>();

        Category category;
        CategoryDTO categoryDTO;

        private Category GetCategory()
        {
            return new Category()
            {
                Id = 1,
                Name = "CPU",
                Description = "Central Processing Units"
            };
        }

        private CategoryDTO GetCategoryDTO()
        {
            return new CategoryDTO()
            {
                Id = 1,
                Name = "CPU",
                Description = "Central Processing Units"
            };
        }

        private void SetupMocks()
        {
            categoryRepo = categoryRepositoryMock.Object;
            mapper = mapperMock.Object;
        }

        private void SetupCategoryDTOMock()
        {
            category = GetCategory();
            mapperMock.Setup(o => o.Map<CategoryDTO>(category)).Returns(GetCategoryDTO());
        }

        private void SetupCategoryEntityMock()
        {
            categoryDTO = GetCategoryDTO();
            mapperMock.Setup(o => o.Map<Category>(categoryDTO)).Returns(GetCategory());
        }

        [Fact]
        public void GetCategoryById_WithValidId_Returns_ExpectedCategory()
        {
            // Arrange
            category = GetCategory();
            SetupMocks();
            SetupCategoryDTOMock();

            categoryRepositoryMock.Setup(o => o.GetCategoryById(It.IsAny<int>())).Returns(category);

            var categoryService = new CategoryService(categoryRepo, mapper);
            int id = 1;

            // Act
            var result = categoryService.GetCategoryById(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("CPU", result.Name);
            categoryRepositoryMock.Verify(repo => repo.GetCategoryById(id), Times.Once);
        }

        [Fact]
        public void AddCategory_WithValidCategoryProvided_ReturnsAddedCategory()
        {
            // Arrange
            categoryDTO = GetCategoryDTO();
            category = GetCategory();

            categoryRepositoryMock.Setup(o => o.AddCategory(It.IsAny<Category>()))
                .Callback<Category>(c => c.Id = 1);

            mapperMock.Setup(mapper => mapper.Map<Category>(categoryDTO)).Returns(category);
            mapperMock.Setup(mapper => mapper.Map<CategoryDTO>(category)).Returns(categoryDTO);

            var categoryService = new CategoryService(categoryRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = categoryService.AddCategory(categoryDTO);

            // Assert
            Assert.Equal(categoryDTO.Name, result.Name);
            Assert.Equal(categoryDTO.Description, result.Description);
            categoryRepositoryMock.Verify(repo => repo.AddCategory(category), Times.Once);
        }

        [Fact]
        public void UpdateCategory_WithValidCategoryDetailsProvided_ReturnsUpdatedCategory()
        {
            // Arrange
            categoryDTO = GetCategoryDTO();
            category = GetCategory();

            var updatedCategory = new Category
            {
                Id = 1,
                Name = "Updated CPU",
                Description = "Updated Description"
            };

            var updatedCategoryDTO = new CategoryDTO
            {
                Id = 1,
                Name = "Updated CPU",
                Description = "Updated Description"
            };

            mapperMock.Setup(mapper => mapper.Map<Category>(categoryDTO)).Returns(category);
            mapperMock.Setup(mapper => mapper.Map<CategoryDTO>(updatedCategory)).Returns(updatedCategoryDTO);

            categoryRepositoryMock.Setup(o => o.UpdateCategory(It.IsAny<Category>()))
                .Returns(updatedCategory);

            var categoryService = new CategoryService(categoryRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = categoryService.UpdateCategory(categoryDTO);

            // Assert
            Assert.Equal("Updated CPU", result.Name);
            Assert.Equal("Updated Description", result.Description);
            categoryRepositoryMock.Verify(repo => repo.UpdateCategory(category), Times.Once);
        }
    }
}