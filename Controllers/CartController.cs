using krist_server.DTO.CartDTOs;
using krist_server.DTO.ResponseDTO;
using krist_server.Interfaces;
using krist_server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace krist_server.Controllers
{
  [ApiController]
  [Route("api/cart")]
  public class CartController : ControllerBase
  {
    private readonly ICartRepository _cartRepo;

    public CartController(ICartRepository cartRepo)
    {
      _cartRepo = cartRepo;
    }

    [Authorize]
    [HttpPost("add")]
    public async Task<ActionResult<Cart>> AddToCart([FromBody] AddToCartDTO addToCartDto)
    {
      try
      {
        var cartItem = await _cartRepo.AddToCartAsync(addToCartDto.UserId, addToCartDto);

        if (cartItem == null)
        {
          return BadRequest(new ErrorResponseDTO("Failed to add item to cart", 400));
        }

        return Ok(JsonConvert.SerializeObject(cartItem));
      }
      catch (Exception e)
      {
        Console.WriteLine("Error in 'AddToCart' : " + e.Message);
        return BadRequest(new ErrorResponseDTO("Internal Server Error", 500));
      }

    }

    [Authorize]
    [HttpGet("{userId}")]
    public async Task<ActionResult<List<Cart>>> GetCartByUserId([FromRoute] string userId)
    {
      var cartItems = await _cartRepo.GetCartByUserIdAsync(userId);
      if (cartItems == null || cartItems.Count == 0)
      {
        List<Cart> emptyList = [];
        return Ok(emptyList);
      }

      return Ok(JsonConvert.SerializeObject(cartItems));
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartDTO updateCartDto)
    {
      System.Console.WriteLine("UpdateCartDTO " + JsonConvert.SerializeObject(updateCartDto));
      var updatedCartItem = await _cartRepo.UpdateCartItemAsync(updateCartDto.CartId, updateCartDto.Quantity);

      if (updatedCartItem == null)
      {
        return BadRequest("Failed to update cart item");
      }

      return Ok(new MutationResponseDTO("Cart item updated successfully"));
    }

    [HttpDelete("{cartId}")]
    public async Task<IActionResult> DeleteCartItem([FromRoute] string cartId)
    {
      var result = await _cartRepo.DeleteCartItemAsync(cartId);

      if (!result)
        return NotFound(new ErrorResponseDTO("Cart item not found", 404));

      return Ok(new MutationResponseDTO("Cart item deleted successfully"));
    }

    [HttpDelete("clear/{userId}")]
    public async Task<IActionResult> ClearCart(string userId)
    {
      var result = await _cartRepo.ClearCartAsync(userId);
      if (!result)
        return BadRequest("Failed to clear cart");

      return Ok(new MutationResponseDTO("Cart cleared successfully"));
    }
  }
}
