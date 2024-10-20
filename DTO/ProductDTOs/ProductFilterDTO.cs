namespace krist_server.DTO.ProductDTOs
{
    public class ProductFilterDto
    {
        public List<string> Categories { get; set; } = [];
        public List<string> Colors { get; set; } = [];
        public List<string> Sizes { get; set; } = [];
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 1000;
        public string SortBy { get; set; } = "featured";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 16;
    }
}
