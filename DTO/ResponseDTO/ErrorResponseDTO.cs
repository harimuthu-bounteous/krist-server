namespace krist_server.DTO.ResponseDTO
{
    public class ErrorResponseDTO
    {
        public string ErrorMessage { get; set; }
        public int StatusCode { get; set; }

        public ErrorResponseDTO(string ErrorMessage, int StatusCode)
        {
            this.ErrorMessage = ErrorMessage;
            this.StatusCode = StatusCode;
        }
    }

}