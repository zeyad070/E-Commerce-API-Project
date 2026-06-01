using Microsoft.AspNetCore.Http;

namespace ProductApp.BLL
{
    public interface IImageManager
    {
        Task<string> UploadAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default);
        Task<string> UpdateProductImageAsync(int productId, IFormFile file, CancellationToken cancellationToken = default);
        Task<string> UpdateCategoryImageAsync(int categoryId, IFormFile file, CancellationToken cancellationToken = default);
    }
}
