using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using krist_server.Models;

namespace krist_server.DTO.CartDTOs
{
    public class CartWithProductDTO
    {
        public string? CartId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;

        // Product details
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<string>? Colors { get; set; }
        public List<string>? Sizes { get; set; }
        public List<ProductImage>? Images { get; set; }
    }

}