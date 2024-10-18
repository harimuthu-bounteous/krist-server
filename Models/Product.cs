using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace krist_server.Models
{

    [Table("products")]
    public class Product : BaseModel
    {
        [PrimaryKey("product_id", false)]
        public string? ProductId { get; set; } = string.Empty;

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("category")]
        public string Category { get; set; } = string.Empty;

        [Column("price")]
        public decimal Price { get; set; }

        [Column("original_price")]
        public decimal OriginalPrice { get; set; }

        [Column("available_stock")]
        public int AvailableStock { get; set; }

        [Column("rating")]
        public double Rating { get; set; }

        [Column("total_reviews")]
        public int TotalReviews { get; set; }

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("colors")]
        public List<string>? Colors { get; set; }

        [Column("sizes")]
        public List<string>? Sizes { get; set; }

        [Column("images")]
        public List<ProductImage> Images { get; set; } = [];
    }
}