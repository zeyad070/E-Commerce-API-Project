using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApp.BLL;
using ProductApp.Common.Utilities;

namespace ProductSystem.APIs.Controllers
{
    [Route("api")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageManager _imageManager;

        public ImagesController(IImageManager imageManager)
        {
            _imageManager = imageManager;
        }

        [HttpPost("image/upload")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<string>>> Upload(IFormFile file)
        {
            try
            {
                var url = await _imageManager.UploadAsync(file, "general");
                return Ok(ApiResponse<string>.Ok(url, "Image uploaded successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.BadRequest(ex.Message));
            }
        }

        [HttpPost("products/{id:int}/image")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<string>>> UploadProductImage(int id, IFormFile file)
        {
            try
            {
                var url = await _imageManager.UpdateProductImageAsync(id, file);
                return Ok(ApiResponse<string>.Ok(url, "Product image updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.BadRequest(ex.Message));
            }
        }

        [HttpPost("categories/{id:int}/image")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<string>>> UploadCategoryImage(int id, IFormFile file)
        {
            try
            {
                var url = await _imageManager.UpdateCategoryImageAsync(id, file);
                return Ok(ApiResponse<string>.Ok(url, "Category image updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.BadRequest(ex.Message));
            }
        }
    }
}
