using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace krist_server.Models
{
    [Table("cart_items")]
    public class Cart : BaseModel
    {
        [PrimaryKey("cart_id", false)]
        public string? CartId { get; set; } = string.Empty;

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("color")]
        public string Color { get; set; } = string.Empty;

        [Column("size")]
        public string Size { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // [Reference(typeof(Product), ReferenceAttribute.JoinType.Inner, true, "product_id", "ProductId")]
        // public Product Product { get; set; } = new Product();
    }
}