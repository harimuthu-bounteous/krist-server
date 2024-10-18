using krist_server.DTO.CartDTOs;
using krist_server.Interfaces;
using krist_server.Models;
using Newtonsoft.Json;
using Supabase;
using Supabase.Postgrest.Exceptions;

namespace krist_server.Repository
{
  public class CartRepository : ICartRepository
  {
    private readonly Client _client;

    public CartRepository(Client client)
    {
      _client = client;
    }

    // Add item to cart
    public async Task<Cart?> AddToCartAsync(string userId, AddToCartDTO addToCartDto)
    {
      try
      {
        // Check if product exists
        var product = await _client.From<Product>().Where(p => p.ProductId == addToCartDto.ProductId).Single();
        if (product == null) return null;

        // Validate stock and product attributes
        if (product.AvailableStock < addToCartDto.Quantity) return null;
        if (!product.Colors.Contains(addToCartDto.Color) || !product.Sizes.Contains(addToCartDto.Size)) return null;


        // Console.WriteLine("Products : " + JsonConvert.SerializeObject(product));
        // Console.WriteLine();

        // Check for existing cart item
        var existingCartItem = await _client.From<Cart>()
            .Where(c => c.UserId == userId)
            .Where(c => c.ProductId == addToCartDto.ProductId)
            .Where(c => c.Color == addToCartDto.Color)
            .Where(c => c.Size == addToCartDto.Size)
            .Single();

        // Console.WriteLine("Existing Cart : " + JsonConvert.SerializeObject(existingCartItem));

        if (existingCartItem != null)
        {
          // Update existing cart item quantity
          existingCartItem.Quantity += addToCartDto.Quantity;
          var updatedCartItem = await _client.From<Cart>().Where(c => c.CartId == existingCartItem.CartId).Update(existingCartItem);
          return updatedCartItem.Model;
        }

        // Create new cart item
        var cartItem = new Cart
        {
          UserId = userId,
          ProductId = addToCartDto.ProductId,
          Quantity = addToCartDto.Quantity,
          Color = addToCartDto.Color,
          Size = addToCartDto.Size
        };

        // Console.WriteLine("Cart : " + JsonConvert.SerializeObject(cartItem));

        var createdCartItem = await _client.From<Cart>().Insert(cartItem);
        return createdCartItem.Model;
      }
      catch (PostgrestException ex)
      {
        Console.WriteLine($"PostgrestException: {ex.Message}");
        return null;
      }
    }

    // Get cart by user ID
    public async Task<List<CartWithProductDTO>> GetCartByUserIdAsync(string userId)
    {
      // Fetch all cart items for the user
      var cartItems = await _client.From<Cart>()
          .Where(c => c.UserId == userId)
          .Get();

      // Initialize the response list
      List<CartWithProductDTO> cartWithProducts = [];

      // Loop through each cart item and fetch the corresponding product details
      foreach (var cartItem in cartItems.Models)
      {
        // Fetch product details based on the ProductId
        var product = await _client.From<Product>()
            .Where(p => p.ProductId == cartItem.ProductId)
            .Single();

        // Create a CartWithProductDTO and populate it with cart and product details
        var cartWithProduct = new CartWithProductDTO
        {
          CartId = cartItem.CartId,
          UserId = cartItem.UserId,
          ProductId = cartItem.ProductId,
          Quantity = cartItem.Quantity,
          Color = cartItem.Color,
          Size = cartItem.Size,

          // Product details
          ProductName = product?.Name ?? string.Empty,
          Category = product?.Category ?? string.Empty,
          Price = product?.Price ?? 0,
          Colors = product?.Colors,
          Sizes = product?.Sizes,
          Images = product?.Images
        };

        // Add to the response list
        cartWithProducts.Add(cartWithProduct);
      }

      return cartWithProducts;
    }


    // Update cart item
    public async Task<Cart?> UpdateCartItemAsync(string cartId, int newQuantity)
    {
      try
      {
        var cartItem = await _client.From<Cart>().Where(c => c.CartId == cartId).Single();
        if (cartItem == null) return null;

        cartItem.Quantity = newQuantity;
        var updatedCartItem = await _client.From<Cart>().Where(c => c.CartId == cartId).Update(cartItem);

        return updatedCartItem.Model;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error updating cart item: {ex.Message}");
        return null;
      }
    }

    // Delete cart item
    public async Task<bool> DeleteCartItemAsync(string cartId)
    {
      try
      {
        await _client.From<Cart>().Where(c => c.CartId == cartId).Delete();
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    // Clear entire cart for a user
    public async Task<bool> ClearCartAsync(string userId)
    {
      try
      {
        await _client.From<Cart>().Where(c => c.UserId == userId).Delete();
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}
