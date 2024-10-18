namespace krist_server.DTO.CartDTOs
{
    public class AddToCartDTO
    {

        public string UserId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
    }
}
