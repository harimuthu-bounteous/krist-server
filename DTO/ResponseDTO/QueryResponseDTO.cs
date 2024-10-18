namespace krist_server.DTO.ResponseDTO
{
    public class QueryResponseDTO<T>
    {
        public string Message { get; set; } = "Query successful";
        public T Data { get; set; } // Data could be a single item or a list

        public QueryResponseDTO(T data, string message = "Query successful")
        {
            Data = data;
            Message = message;
        }
    }

}