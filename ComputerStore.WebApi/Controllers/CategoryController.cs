using ComputerStore.Service.DTOs;
using ComputerStore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ComputerStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("GetCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<CategoryDTO>> GetCategories()
        {
            try
            {
                var categories = _categoryService.GetCategories();
                return Ok(categories);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving categories. Please try again later." });
            }
        }

        [HttpGet]
        [Route("GetCategoryById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<CategoryDTO> GetCategoryById(int id)
        {
            try
            {
                var category = _categoryService.GetCategoryById(id);
                if (category == null)
                    return NotFound(new { message = $"Category with ID {id} was not found." });

                return Ok(category);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the category. Please try again later." });
            }
        }

        [HttpGet]
        [Route("GetCategoryByName/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<CategoryDTO> GetCategoryByName(string name)
        {
            try
            {
                var category = _categoryService.GetCategoryByName(name);
                if (category == null)
                    return NotFound(new { message = $"Category with name '{name}' was not found." });

                return Ok(category);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the category. Please try again later." });
            }
        }

        [HttpPost]
        [Route("AddCategory")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<CategoryDTO> AddCategory([FromBody] CategoryDTO categoryDto)
        {
            try
            {
                if (categoryDto == null)
                    return BadRequest(new { message = "Category data cannot be null." });

                if (string.IsNullOrWhiteSpace(categoryDto.Name))
                    return BadRequest(new { message = "Category name is required." });

                // Check if a category with the same name already exists
                var existingCategory = _categoryService.GetCategoryByName(categoryDto.Name);
                if (existingCategory != null)
                    return UnprocessableEntity(new { message = $"A category with the name '{categoryDto.Name}' already exists." });

                var createdCategory = _categoryService.AddCategory(categoryDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while creating the category. Please try again later." });
            }
        }

        [HttpPut]
        [Route("UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<CategoryDTO> UpdateCategory([FromBody] CategoryDTO categoryDto)
        {
            try
            {
                if (categoryDto == null)
                    return BadRequest(new { message = "Category data cannot be null." });

                if (categoryDto.Id <= 0)
                    return BadRequest(new { message = "Valid category ID is required." });

                if (string.IsNullOrWhiteSpace(categoryDto.Name))
                    return BadRequest(new { message = "Category name is required." });

                var existingCategory = _categoryService.GetCategoryByName(categoryDto.Name);
                if (existingCategory != null && existingCategory.Id != categoryDto.Id)
                    return UnprocessableEntity(new { message = $"Another category with the name '{categoryDto.Name}' already exists." });

                var updatedCategory = _categoryService.UpdateCategory(categoryDto);
                if (updatedCategory == null)
                    return NotFound(new { message = $"Category with ID {categoryDto.Id} was not found." });

                return Ok(updatedCategory);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating the category. Please try again later." });
            }
        }

        [HttpDelete]
        [Route("DeleteCategory/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<bool> DeleteCategory(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Valid category ID is required." });

                var deleted = _categoryService.DeleteCategory(id);
                if (!deleted)
                    return NotFound(new { message = $"Category with ID {id} was not found." });

                return Ok(new { message = $"Category with ID {id} was successfully deleted." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the category. Please try again later." });
            }
        }
    }
}