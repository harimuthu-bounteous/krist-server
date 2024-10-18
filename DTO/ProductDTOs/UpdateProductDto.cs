using krist_server.Models;

namespace krist_server.DTO.ProductDTOs
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal OriginalPrice { get; set; }

        public int AvailableStock { get; set; }

        public double Rating { get; set; }

        public int TotalReviews { get; set; }

        public string Description { get; set; } = string.Empty;

        public List<string>? Colors { get; set; }

        public List<string>? Sizes { get; set; }

        public List<ProductImage> Images { get; set; } = [];

    }
}