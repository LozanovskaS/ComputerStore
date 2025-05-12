using ComputerStore.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStore.Data.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetProducts();
        Product GetProductById(int id);
        Product GetProductByName(string name);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        bool DeleteProduct(int id);
        bool UpdateStock(int productId, int quantity);
        List<string> GetProductCategories(int productId);
    }
}
