namespace ProductApp.DAL
{
    public class Category : IAuditEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
