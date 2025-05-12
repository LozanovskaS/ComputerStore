using ComputerStore.Service.DTOs;
using ComputerStore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputerStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        #region Product CRUD Operations

        [HttpGet]
        [Route("GetProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<ProductDTO>> GetProducts()
        {
            try
            {
                var products = _productService.GetProducts();
                return Ok(products);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products. Please try again later." });
            }
        }

        [HttpGet]
        [Route("GetProductById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ProductDTO> GetProductById(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                if (product == null)
                    return NotFound(new { message = $"Product with ID {id} was not found." });

                return Ok(product);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the product. Please try again later." });
            }
        }

        [HttpGet]
        [Route("GetProductByName/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ProductDTO> GetProductByName(string name)
        {
            try
            {
                var product = _productService.GetProductByName(name);
                if (product == null)
                    return NotFound(new { message = $"Product with name '{name}' was not found." });

                return Ok(product);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the product. Please try again later." });
            }
        }

        [HttpPost]
        [Route("AddProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ProductDTO> AddProduct([FromBody] ProductDTO productDto)
        {
            try
            {
                if (productDto == null)
                    return BadRequest(new { message = "Product data cannot be null." });

                if (string.IsNullOrWhiteSpace(productDto.Name))
                    return BadRequest(new { message = "Product name is required." });

                if (productDto.Price <= 0)
                    return BadRequest(new { message = "Product price must be greater than zero." });

                var createdProduct = _productService.AddProduct(productDto);
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the product. Please try again later." });
            }
        }

        [HttpPut]
        [Route("UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ProductDTO> UpdateProduct([FromBody] ProductDTO productDto)
        {
            try
            {
                if (productDto == null)
                    return BadRequest(new { message = "Product data cannot be null." });

                if (productDto.Id <= 0)
                    return BadRequest(new { message = "Valid product ID is required." });

                if (string.IsNullOrWhiteSpace(productDto.Name))
                    return BadRequest(new { message = "Product name is required." });

                if (productDto.Price <= 0)
                    return BadRequest(new { message = "Product price must be greater than zero." });

                var updatedProduct = _productService.UpdateProduct(productDto);
                if (updatedProduct == null)
                    return NotFound(new { message = $"Product with ID {productDto.Id} was not found." });

                return Ok(updatedProduct);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating the product. Please try again later." });
            }
        }

        [HttpDelete]
        [Route("DeleteProduct/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<bool> DeleteProduct(int id)
        {
            try
            {
                var deleted = _productService.DeleteProduct(id);
                if (!deleted)
                    return NotFound(new { message = $"Product with ID {id} was not found." });

                return Ok(new { message = $"Product with ID {id} was successfully deleted." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the product. Please try again later." });
            }
        }

        #endregion

        #region Stock Management

        [HttpPut]
        [Route("UpdateStock/{productId}/{quantity}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ItemDTO> UpdateStock(int productId, int quantity)
        {
            try
            {
                if (quantity < 0)
                    return BadRequest(new { message = "Quantity cannot be negative." });

                var result = _productService.UpdateStock(productId, quantity);
                return Ok(new
                {
                    productId = result.ProductId,
                    quantity = result.Quantity,
                    message = $"Stock for product ID {productId} has been updated to {quantity} units."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while updating stock. Please try again later." });
            }
        }

        [HttpPost]
        [Route("ImportStock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<BasketResultItemDTO>> ImportStockData([FromBody] List<StockImportDTO> stockData)
        {
            try
            {
                if (stockData == null || !stockData.Any())
                    return BadRequest(new { message = "Stock data cannot be null or empty." });

                foreach (var item in stockData)
                {
                    if (string.IsNullOrWhiteSpace(item.Name))
                        return BadRequest(new { message = "Product name is required for all items." });

                    if (item.Price <= 0)
                        return BadRequest(new { message = $"Invalid price for product '{item.Name}'. Price must be greater than zero." });

                    if (item.Quantity <= 0)
                        return BadRequest(new { message = $"Invalid quantity for product '{item.Name}'. Quantity must be greater than zero." });

                    if (item.Categories == null || !item.Categories.Any())
                        return BadRequest(new { message = $"At least one category is required for product '{item.Name}'." });
                }

                var importResults = _productService.ImportStockData(stockData);
                return Ok(new
                {
                    data = importResults,
                    message = $"Successfully imported {importResults.Count} products."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error importing stock data: {ex.Message}" });
            }
        }

        #endregion

        #region Basket Calculation

        [HttpPost]
        [Route("CalculateBasket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<BasketCalculationResultDTO> CalculateBasket([FromBody] List<ItemDTO> basketItems)
        {
            try
            {
                if (basketItems == null || !basketItems.Any())
                    return BadRequest(new { message = "Basket cannot be empty." });

                foreach (var item in basketItems)
                {
                    if (item.ProductId <= 0)
                        return BadRequest(new { message = "Invalid product ID in basket." });

                    if (item.Quantity <= 0)
                        return BadRequest(new { message = $"Invalid quantity for product ID {item.ProductId}. Quantity must be greater than zero." });
                }

                var calculationResult = _productService.CalculateBasket(basketItems);
                return Ok(calculationResult);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("stock"))
            {
                return BadRequest(new { message = $"Insufficient stock: {ex.Message}" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error calculating basket: {ex.Message}" });
            }
        }

        #endregion
    }
}