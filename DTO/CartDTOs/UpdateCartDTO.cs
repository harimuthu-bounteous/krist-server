namespace krist_server.DTO.CartDTOs
{
    public class UpdateCartDTO
    {
        public string CartId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
