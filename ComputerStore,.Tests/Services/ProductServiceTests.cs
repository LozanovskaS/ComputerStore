using AutoMapper;
using ComputerStore.Data.Entities;
using ComputerStore.Data.Interfaces;
using ComputerStore.Service.DTOs;
using ComputerStore.Service.Services;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ComputerStore.Tests.Services
{
    public class ProductServiceTests
    {
        IProductRepository productRepo;
        ICategoryRepository categoryRepo;
        IMapper mapper;
        Mock<IProductRepository> productRepositoryMock = new Mock<IProductRepository>();
        Mock<ICategoryRepository> categoryRepositoryMock = new Mock<ICategoryRepository>();
        Mock<IMapper> mapperMock = new Mock<IMapper>();

        Product product;
        ProductDTO productDTO;
        Category category;

        private Product GetProduct()
        {
            return new Product()
            {
                Id = 1,
                Name = "Intel Core i9-9900K",
                Description = "High-performance desktop processor",
                Price = 475.99m,
                Quantity = 10,
                Category = new List<Category>() { GetCategory() }
            };
        }

        private ProductDTO GetProductDTO()
        {
            return new ProductDTO()
            {
                Id = 1,
                Name = "Intel Core i9-9900K",
                Description = "High-performance desktop processor",
                Price = 475.99m,
                Quantity = 10,
                Categories = new List<CategoryDTO>() { GetCategoryDTO() }
            };
        }

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
            productRepo = productRepositoryMock.Object;
            categoryRepo = categoryRepositoryMock.Object;
            mapper = mapperMock.Object;
        }

        private void SetupProductDTOMock()
        {
            product = GetProduct();
            mapperMock.Setup(o => o.Map<ProductDTO>(product)).Returns(GetProductDTO());
        }

        private void SetupProductEntityMock()
        {
            productDTO = GetProductDTO();
            mapperMock.Setup(o => o.Map<Product>(productDTO)).Returns(GetProduct());
        }

        [Fact]
        public void AddProduct_WithValidProductProvided_ReturnsAddedProduct()
        {
            // Arrange
            productDTO = GetProductDTO();
            product = GetProduct();

            SetupMocks();
            SetupProductEntityMock();

            var resultProductDTO = GetProductDTO();
            mapperMock
                .Setup(o => o.Map<ProductDTO>(It.IsAny<Product>()))
                .Returns(resultProductDTO);

            productRepositoryMock
                .Setup(o => o.AddProduct(It.IsAny<Product>()))
                .Callback<Product>(p => {
                    p.Id = 1;
                });

            var productService = new ProductService(productRepo, categoryRepo, mapper);

            // Act
            var result = productService.AddProduct(productDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productDTO.Name, result.Name);
            Assert.Equal(productDTO.Price, result.Price);
            productRepositoryMock.Verify(repo => repo.AddProduct(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public void UpdateProduct_WithValidProductDetailsProvided_ReturnsUpdatedProduct()
        {
            // Arrange
            productDTO = GetProductDTO();
            productDTO.Name = "Updated Product";
            productDTO.Price = 499.99m;

            product = GetProduct();
            product.Name = "Updated Product";
            product.Price = 499.99m;

            var updatedProduct = new Product
            {
                Id = 1,
                Name = "Updated Product",
                Description = "High-performance desktop processor",
                Price = 499.99m,
                Quantity = 10,
                Category = new List<Category>() { GetCategory() }
            };

            SetupMocks();

            mapperMock.Setup(o => o.Map<Product>(productDTO)).Returns(product);
            mapperMock.Setup(o => o.Map<ProductDTO>(updatedProduct)).Returns(productDTO);

            productRepositoryMock.Setup(o => o.UpdateProduct(It.IsAny<Product>()));
            productRepositoryMock.Setup(o => o.GetProductById(1)).Returns(updatedProduct);

            var productService = new ProductService(productRepo, categoryRepo, mapper);

            // Act
            var result = productService.UpdateProduct(productDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Product", result.Name);
            Assert.Equal(499.99m, result.Price);
            productRepositoryMock.Verify(repo => repo.UpdateProduct(It.IsAny<Product>()), Times.Once);
            productRepositoryMock.Verify(repo => repo.GetProductById(1), Times.Once);
        }

        [Fact]
        public void CalculateBasket_MultipleProductsSameCategory_AppliesDiscount()
        {
            // Arrange
            SetupMocks();

            var category = GetCategory();

            var product1 = new Product
            {
                Id = 1,
                Name = "Intel Core i9-9900K",
                Description = "High-performance desktop processor",
                Price = 475.99m,
                Quantity = 10,
                Category = new List<Category> { category }
            };

            var product2 = new Product
            {
                Id = 2,
                Name = "AMD Ryzen 9 5950X",
                Description = "High-performance desktop processor",
                Price = 549.99m,
                Quantity = 15,
                Category = new List<Category> { category }
            };

            var basketItems = new List<ItemDTO>
            {
                new ItemDTO { ProductId = 1, Quantity = 1 },
                new ItemDTO { ProductId = 2, Quantity = 1 }
            };

            productRepositoryMock.Setup(o => o.GetProductById(1)).Returns(product1);
            productRepositoryMock.Setup(o => o.GetProductById(2)).Returns(product2);

            var productService = new ProductService(productRepo, categoryRepo, mapper);

            // Act
            var result = productService.CalculateBasket(basketItems);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(1025.98m, result.TotalPrice);

            var discountedItem = result.Items.FirstOrDefault(i => i.Discount > 0);
            Assert.NotNull(discountedItem);
            Assert.Equal(5m, discountedItem.Discount);

            Assert.Equal(23.7995m, result.TotalDiscount, 4);
            Assert.Equal(1002.1805m, result.FinalPrice, 4);
        }
    }
}