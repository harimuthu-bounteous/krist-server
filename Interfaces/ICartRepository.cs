using krist_server.DTO.CartDTOs;
using krist_server.Models;

namespace krist_server.Interfaces
{
  public interface ICartRepository
  {
    Task<Cart?> AddToCartAsync(string userId, AddToCartDTO addToCartDto);
    Task<List<CartWithProductDTO>> GetCartByUserIdAsync(string userId);
    Task<Cart?> UpdateCartItemAsync(string cartId, int newQuantity);
    Task<bool> DeleteCartItemAsync(string cartId);
    Task<bool> ClearCartAsync(string userId);
  }
}
