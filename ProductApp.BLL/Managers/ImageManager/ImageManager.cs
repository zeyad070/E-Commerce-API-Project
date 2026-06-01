using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ProductApp.Common.Utilities;
using ProductApp.DAL;

namespace ProductApp.BLL
{
    public class ImageManager : IImageManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ImageSettings _imageSettings;
        private readonly string _uploadsFolder;

        public ImageManager(IUnitOfWork unitOfWork, IOptions<ImageSettings> imageSettings)
        {
            _unitOfWork = unitOfWork;
            _imageSettings = imageSettings.Value;
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        }

        public async Task<string> UploadAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default)
        {
            ValidateFile(file);

            var folderPath = Path.Combine(_uploadsFolder, folderName);
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            return $"/uploads/{folderName}/{fileName}";
        }

        public async Task<string> UpdateProductImageAsync(int productId, IFormFile file, CancellationToken cancellationToken = default)
        {
            ValidateFile(file);

            var product = await _unitOfWork.ProductRepositories.GetByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productId} not found");

            var imageUrl = await UploadAsync(file, "products", cancellationToken);

            if (!string.IsNullOrEmpty(product.ImageUrl))
                DeleteOldFile(product.ImageUrl);

            product.ImageUrl = imageUrl;
            await _unitOfWork.SaveChangesAsync();

            return imageUrl;
        }

        public async Task<string> UpdateCategoryImageAsync(int categoryId, IFormFile file, CancellationToken cancellationToken = default)
        {
            ValidateFile(file);

            var category = await _unitOfWork.CategoryRepositories.GetByIdAsync(categoryId);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {categoryId} not found");

            var imageUrl = await UploadAsync(file, "categories", cancellationToken);

            if (!string.IsNullOrEmpty(category.ImageUrl))
                DeleteOldFile(category.ImageUrl);

            category.ImageUrl = imageUrl;
            await _unitOfWork.SaveChangesAsync();

            return imageUrl;
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_imageSettings.AllowedExtensions.Contains(extension))
                throw new InvalidOperationException($"Invalid file type. Allowed: {string.Join(", ", _imageSettings.AllowedExtensions)}");

            var maxSizeBytes = _imageSettings.MaxFileSizeMB * 1024 * 1024;
            if (file.Length > maxSizeBytes)
                throw new InvalidOperationException($"File size exceeds {_imageSettings.MaxFileSizeMB} MB limit");

            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedMimeTypes.Contains(file.ContentType?.ToLowerInvariant()))
                throw new InvalidOperationException("Invalid file MIME type. Allowed: jpeg, png, gif, webp");
        }

        private static void DeleteOldFile(string imageUrl)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
