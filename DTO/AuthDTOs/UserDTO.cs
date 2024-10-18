namespace krist_server.DTO.AuthDTOs
{
    public class UserDTO
    {
        public string? UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; } = string.Empty;
    }
}