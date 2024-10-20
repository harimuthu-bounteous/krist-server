using AutoMapper;
using krist_server.DTO.ProductDTOs;
using krist_server.Interfaces;
using krist_server.Models;
using Supabase;
using Supabase.Postgrest.Exceptions;

namespace krist_server.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly Client _client;
        private readonly IMapper _mapper;

        public ProductRepository(Client client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            var products = await _client.From<Product>().Get();
            var productDtos = _mapper.Map<List<ProductDto>>(products.Models);
            return productDtos;
        }

        public async Task<List<ProductDto>> GetFilteredProductsAsync(ProductFilterDto filters)
        {
            try
            {
                if (
                    filters.Categories.Count == 0 &&
                    filters.Colors.Count == 0 &&
                    filters.Sizes.Count == 0 &&
                    filters.MaxPrice == 1000 &&
                    filters.MinPrice == 0 &&
                    filters.SortBy == "featured" &&
                    filters.PageSize == 16 &&
                    filters.PageNumber == 1
                )
                {
                    var allProducts = await _client.From<Product>()
                               .Order(x => x.TotalReviews, Supabase.Postgrest.Constants.Ordering.Descending)
                               .Get();
                    return _mapper.Map<List<ProductDto>>(allProducts.Models);
                }

                // Build query
                var query = _client.From<Product>();

                // Apply filters
                if (filters.Categories.Count > 0)
                    query = (Supabase.Interfaces.ISupabaseTable<Product, Supabase.Realtime.RealtimeChannel>)query.Filter(x => x.Category, Supabase.Postgrest.Constants.Operator.In, filters.Categories);

                if (filters.Colors.Count > 0)
                    query = (Supabase.Interfaces.ISupabaseTable<Product, Supabase.Realtime.RealtimeChannel>)query.Filter(x => x.Colors, Supabase.Postgrest.Constants.Operator.Overlap, filters.Colors);

                if (filters.Sizes.Count > 0)
                    query = (Supabase.Interfaces.ISupabaseTable<Product, Supabase.Realtime.RealtimeChannel>)query.Filter(x => x.Sizes, Supabase.Postgrest.Constants.Operator.Overlap, filters.Sizes);

                if (filters.MaxPrice <= 1000 && filters.MinPrice >= 0)
                {
                    query = (Supabase.Interfaces.ISupabaseTable<Product, Supabase.Realtime.RealtimeChannel>)query.Where(x => x.Price <= filters.MaxPrice && x.Price >= filters.MinPrice);
                }

                if (filters.SortBy == "price-asc")
                    query = (Supabase.Interfaces.ISupabaseTable<Product, Supabase.Realtime.RealtimeChannel>)query.Order(x => x.Price, Supabase.Postgrest.Constants.Ordering.Ascending);
                else if (filters.SortBy == "price-desc")
                    query = (Supabase.Interfaces.ISupabaseTable<Product, Supabase.Realtime.RealtimeChannel>)query.Order(x => x.Price, Supabase.Postgrest.Constants.Ordering.Descending);
                else
                    query = (Supabase.Interfaces.ISupabaseTable<Product, Supabase.Realtime.RealtimeChannel>)query.Order(x => x.TotalReviews, Supabase.Postgrest.Constants.Ordering.Descending); // Default sort

                var products = await query.Get();
                return _mapper.Map<List<ProductDto>>(products.Models);
            }
            catch (PostgrestException pe)
            {
                Console.WriteLine("PostgrestException in 'GetFilteredProductsAsync': " + pe.Message);
                return [];
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in 'GetFilteredProductsAsync': " + e.Message);
                return [];
            }
        }


        public async Task<Product?> GetProductByIdAsync(string id)
        {
            var response = await _client.From<Product>().Where(x => x.ProductId == id).Single();
            return response;
        }

        public async Task<bool> DoesProductExist(string id)
        {
            var product = await _client.From<Product>().Where(p => p.ProductId == id).Single();
            return product != null;
        }

        public async Task<ProductDto?> CreateProductAsync(CreateProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);

                var createdProduct = await _client.From<Product>().Insert(product);
                if (createdProduct == null)
                    return null;

                return _mapper.Map<ProductDto>(createdProduct.Model);

            }
            catch (PostgrestException ex)
            {
                Console.WriteLine($"PostgrestException: {ex.Message}");
                return null;
            }

        }

        public async Task<Product?> UpdateProductAsync(string id, Product product)
        {
            var response = await _client.From<Product>().Where(x => x.ProductId == id).Update(product);
            return response.Model;
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            try
            {
                await _client.From<Product>().Where(x => x.ProductId == id).Delete();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Product>> GetRelatedProducts(string productId)
        {
            var currentProduct = await _client.From<Product>().Where(p => p.ProductId == productId).Single();
            if (currentProduct == null) return [];

            var relatedProducts = await _client.From<Product>()
                .Where(p => p.Category == currentProduct.Category && p.ProductId != currentProduct.ProductId)
                .Limit(10)
                .Get();

            return relatedProducts.Models;
        }
    }
}