using krist_server.DTO.ProductDTOs;
using krist_server.Models;

namespace krist_server.Interfaces
{
  public interface IProductRepository
  {
    Task<List<ProductDto>> GetProductsAsync();
    Task<List<ProductDto>> GetFilteredProductsAsync(ProductFilterDto filters);
    Task<Product?> GetProductByIdAsync(string id);
    Task<bool> DoesProductExist(string id);
    Task<ProductDto?> CreateProductAsync(CreateProductDto product);
    Task<Product?> UpdateProductAsync(string id, Product product);
    Task<bool> DeleteProductAsync(string id);
    Task<List<Product>> GetRelatedProducts(string productId);
  }
}