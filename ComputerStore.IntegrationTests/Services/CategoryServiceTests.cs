using ComputerStore.IntegrationTests.Base;
using ComputerStore.Service.DTOs;
using ComputerStore.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace ComputerStore.IntegrationTests.Services
{
    public class CategoryServiceTests : IntegrationTestBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryServiceTests()
        {
            _categoryService = ServiceProvider.GetRequiredService<ICategoryService>();
        }

        [Fact]
        public void GetCategories_ReturnsAllCategories()
        {
            // Act
            var categories = _categoryService.GetCategories();

            // Assert
            Assert.Equal(2, categories.Count);
            Assert.Contains(categories, c => c.Name == "CPU");
            Assert.Contains(categories, c => c.Name == "Storage");
        }

        [Fact]
        public void AddCategory_WithValidCategory_AddsToDatabase()
        {
            // Arrange
            var newCategoryDto = new CategoryDTO
            {
                Name = "GPU",
                Description = "Graphics Processing Units"
            };

            // Act
            var result = _categoryService.AddCategory(newCategoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newCategoryDto.Name, result.Name);
            Assert.Equal(newCategoryDto.Description, result.Description);

            // Verify in database
            var categories = _categoryService.GetCategories();
            Assert.Equal(3, categories.Count);
            Assert.Contains(categories, c => c.Name == "GPU");
        }

        [Fact]
        public void DeleteCategory_WithValidId_RemovesFromDatabase()
        {
            // Act
            var result = _categoryService.DeleteCategory(2);

            // Assert
            Assert.True(result);

            // Verify in database
            var categories = _categoryService.GetCategories();
            Assert.Single(categories);
            Assert.DoesNotContain(categories, c => c.Id == 2);
        }
    }
}