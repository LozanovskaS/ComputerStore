using ComputerStore.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStore.Service.Interfaces
{
    public interface IProductService
    {
        List<ProductDTO> GetProducts();
        ProductDTO GetProductById(int id);
        ProductDTO GetProductByName(string name);
        ProductDTO AddProduct(ProductDTO product);
        ProductDTO UpdateProduct(ProductDTO product);
        bool DeleteProduct(int id);
        ItemDTO UpdateStock(int productId, int quantity);
        List<BasketResultItemDTO> ImportStockData(List<StockImportDTO> stockData);

        BasketCalculationResultDTO CalculateBasket(List<ItemDTO> basketItems);
    }
}
