namespace krist_server.DTO.ResponseDTO
{
    public class MutationResponseDTO
    {
        public string Message { get; set; }
        public object? Data { get; set; } // Optional data (e.g., created or updated entity)

        public MutationResponseDTO(string message, object? data = null)
        {
            Message = message;
            Data = data;
        }
    }

}