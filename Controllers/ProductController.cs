using krist_server.DTO.ProductDTOs;
using krist_server.DTO.ResponseDTO;
using krist_server.Interfaces;
using krist_server.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace krist_server.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepo;

        public ProductsController(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        [HttpGet]
        public async Task<ActionResult<Product>> GetAllProducts()
        {
            var products = await _productRepo.GetProductsAsync();
            return Ok(JsonConvert.SerializeObject(products));
        }

        [HttpPost("filter")]
        public async Task<ActionResult<List<ProductDto>>> GetFilteredProducts([FromBody] ProductFilterDto filters)
        {
            Console.WriteLine("Filters : " + JsonConvert.SerializeObject(filters));
            var products = await _productRepo.GetFilteredProductsAsync(filters);
            var totalNumberOfProducts = products.Count;

            if (products == null || totalNumberOfProducts == 0)
                return Ok(JsonConvert.SerializeObject(new { paginatedProducts = new List<ProductDto>(), totalNumberOfProducts = 0 }));


            int skipNumber = (filters.PageNumber - 1) * filters.PageSize;
            var paginatedProducts = products.Skip(skipNumber).Take(filters.PageSize).ToList();
            // Console.WriteLine(paginatedProducts.Count);
            return Ok(JsonConvert.SerializeObject(new { paginatedProducts, totalNumberOfProducts }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById([FromRoute] string id)
        {
            var product = await _productRepo.GetProductByIdAsync(id);
            if (product is null)
                return NotFound("Product not found");
            return Ok(JsonConvert.SerializeObject(product));
        }


        [HttpPost]
        public async Task<ActionResult<MutationResponseDTO>> CreateProduct(CreateProductDto productDto)
        {
            var createdProduct = await _productRepo.CreateProductAsync(productDto);

            if (createdProduct == null)
            {
                return BadRequest(new MutationResponseDTO("Failed to create product"));
            }

            var response = new MutationResponseDTO("Product created successfully");

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = createdProduct.ProductId },
                response
            );
        }

        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateProduct([FromRoute] string id, [FromBody] UpdateProductDto productDto)
        // {
        //     if (!await _productRepo.DoesProductExist(id))
        //         return NotFound("Product not found!!");

        //     var updatedProduct = await _productRepo.UpdateProductAsync(productDto, id);
        //     if (updatedProduct == null)
        //         return BadRequest("Failed to update the product");

        //     var successResponse = new MutationResponseDTO("Product updated successfully");
        //     return Ok(successResponse);
        // }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {

            var result = await _productRepo.DeleteProductAsync(id);
            if (!result)
                return NotFound(new ErrorResponseDTO("Product not found!", StatusCodes.Status404NotFound));

            var successResponse = new MutationResponseDTO("Product deleted successfully");
            return Ok(successResponse);
        }

        [HttpGet("{id}/related")]
        public async Task<IActionResult> GetRelatedProducts([FromRoute] string id)
        {
            var relatedProducts = await _productRepo.GetRelatedProducts(id);
            if (relatedProducts == null || relatedProducts.Count == 0)
                return Ok(JsonConvert.SerializeObject(new List<Product>()));

            relatedProducts = relatedProducts.Take(4).ToList();

            return Ok(JsonConvert.SerializeObject(relatedProducts));
        }
    }
}