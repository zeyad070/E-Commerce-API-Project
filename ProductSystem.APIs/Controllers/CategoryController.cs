using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApp.BLL;
using ProductApp.Common.Utilities;

namespace ProductSystem.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryManager _categoryManager;
        private readonly IValidator<CategoryCreateDto> _createValidator;
        private readonly IValidator<CategoryEditDto> _editValidator;

        public CategoryController(
            ICategoryManager categoryManager,
            IValidator<CategoryCreateDto> createValidator,
            IValidator<CategoryEditDto> editValidator)
        {
            _categoryManager = categoryManager;
            _createValidator = createValidator;
            _editValidator = editValidator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CategoryReadDto>>>> GetAll()
        {
            var categories = await _categoryManager.GetAllCategoriesAsync();
            return Ok(ApiResponse<List<CategoryReadDto>>.Ok(categories));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<CategoryReadDto>>> GetById(int id)
        {
            var category = await _categoryManager.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(ApiResponse<CategoryReadDto>.NotFound($"Category with ID {id} not found"));

            return Ok(ApiResponse<CategoryReadDto>.Ok(category));
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<int>>> CreateCategory([FromBody] CategoryCreateDto categoryCreateDto)
        {
            var validation = await _createValidator.ValidateAsync(categoryCreateDto);
            if (!validation.IsValid)
                return BadRequest(ApiResponse<int>.BadRequest("Validation failed",
                    validation.Errors.Select(e => e.ErrorMessage).ToList()));

            var newId = await _categoryManager.CreateCategoryAsync(categoryCreateDto);
            return CreatedAtAction(nameof(GetById), new { id = newId }, ApiResponse<int>.Created(newId));
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateCategory(int id, [FromBody] CategoryEditDto categoryEditDto)
        {
            if (id != categoryEditDto.Id)
                return BadRequest(ApiResponse<string>.BadRequest("ID in the URL does not match ID in the body"));

            var validation = await _editValidator.ValidateAsync(categoryEditDto);
            if (!validation.IsValid)
                return BadRequest(ApiResponse<string>.BadRequest("Validation failed",
                    validation.Errors.Select(e => e.ErrorMessage).ToList()));

            var existing = await _categoryManager.GetCategoryByIdAsync(id);
            if (existing == null)
                return NotFound(ApiResponse<string>.NotFound($"Category with ID {id} not found"));

            await _categoryManager.UpdateCategoryAsync(categoryEditDto);
            return Ok(ApiResponse<string>.Ok("Category updated successfully"));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCategory(int id)
        {
            var category = await _categoryManager.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(ApiResponse<string>.NotFound($"Category with ID {id} not found"));

            await _categoryManager.DeleteCategoryAsync(id);
            return Ok(ApiResponse<string>.Ok("Category deleted successfully"));
        }
    }
}
