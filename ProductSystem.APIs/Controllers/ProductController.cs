using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApp.BLL;
using ProductApp.Common.Utilities;

namespace ProductSystem.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductManager _productManager;
        private readonly IValidator<ProductCreateDto> _createValidator;
        private readonly IValidator<ProductEditDto> _editValidator;

        public ProductController(
            IProductManager productManager,
            IValidator<ProductCreateDto> createValidator,
            IValidator<ProductEditDto> editValidator)
        {
            _productManager = productManager;
            _createValidator = createValidator;
            _editValidator = editValidator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginationResponse<ProductReadDto>>>> GetAll(
            [FromQuery] int? categoryId,
            [FromQuery] string? name,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var products = await _productManager.GetAllProductsAsync(categoryId, name, pageNumber, pageSize);
            return Ok(ApiResponse<PaginationResponse<ProductReadDto>>.Ok(products));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<ProductReadDto>>> GetById(int id)
        {
            var product = await _productManager.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(ApiResponse<ProductReadDto>.NotFound($"Product with ID {id} not found"));

            return Ok(ApiResponse<ProductReadDto>.Ok(product));
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<int>>> CreateProduct([FromBody] ProductCreateDto dto)
        {
            var result = await _createValidator.ValidateAsync(dto);
            if (!result.IsValid)
                return BadRequest(ApiResponse<int>.BadRequest("Validation failed",
                    result.Errors.Select(e => e.ErrorMessage).ToList()));

            var newId = await _productManager.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newId }, ApiResponse<int>.Created(newId));
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateProduct(int id, [FromBody] ProductEditDto dto)
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<string>.BadRequest("ID in the URL does not match ID in the body"));

            var result = await _editValidator.ValidateAsync(dto);
            if (!result.IsValid)
                return BadRequest(ApiResponse<string>.BadRequest("Validation failed",
                    result.Errors.Select(e => e.ErrorMessage).ToList()));

            var existing = await _productManager.GetProductByIdEditAsync(id);
            if (existing == null)
                return NotFound(ApiResponse<string>.NotFound($"Product with ID {id} not found"));

            await _productManager.UpdateProductAsync(dto);
            return Ok(ApiResponse<string>.Ok("Product updated successfully"));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProduct(int id)
        {
            var product = await _productManager.GetProductByIdEditAsync(id);
            if (product == null)
                return NotFound(ApiResponse<string>.NotFound($"Product with ID {id} not found"));

            await _productManager.DeleteProductAsync(id);
            return Ok(ApiResponse<string>.Ok("Product deleted successfully"));
        }
    }
}
