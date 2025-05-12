using ComputerStore.Data.Entities;
using ComputerStore.IntegrationTests.Base;
using ComputerStore.Service.DTOs;
using ComputerStore.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ComputerStore.IntegrationTests.Services
{
    public class ProductServiceTests : IntegrationTestBase
    {
        private readonly IProductService _productService;

        public ProductServiceTests()
        {
            _productService = ServiceProvider.GetRequiredService<IProductService>();
        }

        [Fact]
        public void GetProducts_ReturnsAllProducts()
        {
            // Act
            var products = _productService.GetProducts();

            // Assert
            Assert.Equal(2, products.Count);
            Assert.Contains(products, p => p.Name == "Intel Core i9-9900K");
            Assert.Contains(products, p => p.Name == "Samsung 970 EVO");
        }

        [Fact]
        public void UpdateStock_WithValidDetails_UpdatesProductStock()
        {
            // Arrange
            int productId = 1;
            int newQuantity = 25;

            // Act
            var result = _productService.UpdateStock(productId, newQuantity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal(newQuantity, result.Quantity);

            var product = _productService.GetProductById(productId);
            Assert.Equal(newQuantity, product.Quantity);
        }

        [Fact]
        public void CalculateBasket_MultipleProducts_AppliesDiscount()
        {
            // Arrange
            var newProductDto = new ProductDTO
            {
                Name = "AMD Ryzen 9 5950X",
                Description = "High-performance desktop processor",
                Price = 549.99m,
                Quantity = 5,
                Categories = new List<CategoryDTO>
                {
                    new CategoryDTO { Id = 1, Name = "CPU", Description = "Central Processing Units" }
                }
            };
            _productService.AddProduct(newProductDto);

            var basketItems = new List<ItemDTO>
            {
                new ItemDTO { ProductId = 1, Quantity = 1 },
                new ItemDTO { ProductId = 3, Quantity = 1 }
            };

            // Act
            var result = _productService.CalculateBasket(basketItems);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(1025.98m, result.TotalPrice);

            var discountedItem = result.Items.FirstOrDefault(i => i.Discount > 0);
            Assert.NotNull(discountedItem);
            Assert.Equal(5m, discountedItem.Discount);

            Assert.True(result.TotalDiscount > 0);
            Assert.True(result.FinalPrice < result.TotalPrice);
        }
    }
}