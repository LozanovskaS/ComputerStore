using AutoMapper;
using ComputerStore.Data.Entities;
using ComputerStore.Data.Interfaces;
using ComputerStore.Service.DTOs;
using ComputerStore.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStore.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public List<ProductDTO> GetProducts()
        {
            var products = _productRepository.GetProducts();
            return _mapper.Map<List<ProductDTO>>(products);
        }

        public ProductDTO GetProductById(int id)
        {
            var product = _productRepository.GetProductById(id);
            return _mapper.Map<ProductDTO>(product);
        }

        public ProductDTO GetProductByName(string name)
        {
            var product = _productRepository.GetProductByName(name);
            return _mapper.Map<ProductDTO>(product);
        }

        public ProductDTO AddProduct(ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            _productRepository.AddProduct(product);

            return _mapper.Map<ProductDTO>(product);
        }

        public ProductDTO UpdateProduct(ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            _productRepository.UpdateProduct(product);

            var updatedProduct = _productRepository.GetProductById(product.Id);
            return _mapper.Map<ProductDTO>(updatedProduct);
        }

        public bool DeleteProduct(int id)
        {
            return _productRepository.DeleteProduct(id);
        }

        #region Stock Management
        public ItemDTO UpdateStock(int productId, int quantity)
        {
            var product = _productRepository.GetProductById(productId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {productId} not found");
            }

            if (quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative");
            }

            var success = _productRepository.UpdateStock(productId, quantity);
            return new ItemDTO
            {
                ProductId = productId,
                Quantity = quantity
            };
        }

        public List<BasketResultItemDTO> ImportStockData(List<StockImportDTO> stockData)
        {
            var resultItems = new List<BasketResultItemDTO>();

            foreach (var item in stockData)
            {
                var resultItem = new BasketResultItemDTO
                {
                    ProductName = item.Name,
                    UnitPrice = item.Price,
                    Quantity = item.Quantity,
                    Discount = 0m
                };

                var product = _productRepository.GetProductByName(item.Name);

                if (product == null)
                {
                    product = new Product
                    {
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Category = new List<Category>()
                    };

                    foreach (var categoryName in item.Categories)
                    {
                        var category = _categoryRepository.GetCategoryByName(categoryName.Trim());
                        if (category == null)
                        {
                            category = new Category { Name = categoryName.Trim() };
                            _categoryRepository.AddCategory(category);
                        }

                        product.Category.Add(category);
                    }

                    _productRepository.AddProduct(product);
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.Description))
                    {
                        product.Description = item.Description;
                        _productRepository.UpdateProduct(product);
                    }
                    else
                    {
                        _productRepository.UpdateStock(product.Id, item.Quantity);
                    }
                }

                resultItem.FinalPrice = (resultItem.UnitPrice - resultItem.Discount) * resultItem.Quantity;
                resultItems.Add(resultItem);
            }

            return resultItems;
        }
        #endregion

        #region Basket Calculation

        public BasketCalculationResultDTO CalculateBasket(List<ItemDTO> basketItems)
        {
            if (basketItems == null || !basketItems.Any())
            {
                throw new ArgumentException("Basket cannot be empty");
            }

            var result = new BasketCalculationResultDTO
            {
                Items = new List<BasketResultItemDTO>(),
                TotalPrice = 0,
                TotalDiscount = 0,
                FinalPrice = 0
            };

            var productDetailsByCategory = new Dictionary<string, List<(Product Product, int Quantity)>>();
            var productDetailsById = new Dictionary<int, (Product Product, int Quantity)>();

            foreach (var item in basketItems)
            {
                var product = _productRepository.GetProductById(item.ProductId);
                if (product == null)
                {
                    throw new ArgumentException($"Product with ID {item.ProductId} not found");
                }

                if (product.Quantity < item.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for product {product.Name}. " +
                        $"Requested: {item.Quantity}, Available: {product.Quantity}"
                    );
                }

                productDetailsById[product.Id] = (product, item.Quantity);

                foreach (var category in product.Category)
                {
                    if (!productDetailsByCategory.ContainsKey(category.Name))
                    {
                        productDetailsByCategory[category.Name] = new List<(Product, int)>();
                    }

                    productDetailsByCategory[category.Name].Add((product, item.Quantity));
                }
            }

            var categoriesWithMultipleProducts = productDetailsByCategory
                .Where(kvp => kvp.Value.Count > 1)
                .Select(kvp => kvp.Key)
                .ToList();

            var firstProductsInCategories = new Dictionary<string, int>();
            foreach (var category in categoriesWithMultipleProducts)
            {
                var productsInCategory = productDetailsByCategory[category];
                if (productsInCategory.Count > 0)
                {
                    firstProductsInCategories[category] = productsInCategory[0].Product.Id;
                }
            }

            foreach (var productDetail in productDetailsById.Values)
            {
                var (product, quantity) = productDetail;
                bool isFirstInAnyCategory = firstProductsInCategories.Values.Contains(product.Id);

                decimal discountPercentage = 0;
                decimal discountAmount = 0;
                decimal finalPrice = 0;

                if (isFirstInAnyCategory)
                {
                    discountPercentage = 5;

                    if (quantity > 0)
                    {
                        decimal firstUnitPrice = product.Price * (1 - (discountPercentage / 100));

                        decimal remainingUnitsPrice = 0;
                        if (quantity > 1)
                        {
                            remainingUnitsPrice = product.Price * (quantity - 1);
                        }

                        finalPrice = firstUnitPrice + remainingUnitsPrice;

                        discountAmount = product.Price * (discountPercentage / 100);
                    }
                }
                else
                {
                    finalPrice = product.Price * quantity;
                }

                var basketResultItem = new BasketResultItemDTO
                {
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = quantity,
                    Discount = discountPercentage,
                    FinalPrice = finalPrice
                };

                result.Items.Add(basketResultItem);
                result.TotalPrice += product.Price * quantity;
                result.TotalDiscount += discountAmount;
            }

            result.FinalPrice = result.TotalPrice - result.TotalDiscount;

            return result;
        }
        #endregion
    }
}
