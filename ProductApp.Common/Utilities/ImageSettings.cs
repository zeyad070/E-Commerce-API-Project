namespace ProductApp.Common.Utilities
{
    public class ImageSettings
    {
        public int MaxFileSizeMB { get; set; } = 5;
        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    }
}
