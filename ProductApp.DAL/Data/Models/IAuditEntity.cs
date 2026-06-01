namespace ProductApp.DAL
{
    public interface IAuditEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
