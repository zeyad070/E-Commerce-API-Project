using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProductApp.DAL
{
    public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public override int SaveChanges()
        {
            AuditLog();
            return base.SaveChanges();
        }

        private void AuditLog()
        {
            var dateTime = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<IAuditEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = dateTime;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = dateTime;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            var createdAt = new DateTime(2026, 5, 10, 17, 12, 15, 908);

            var categories = new List<Category>()
            {
                new Category() { Id = 1, Name = "Laptops", CreatedAt = createdAt },
                new Category() { Id = 2, Name = "PCS", CreatedAt = createdAt },
                new Category() { Id = 3, Name = "Phones & Tablets", CreatedAt = createdAt },
                new Category() { Id = 4, Name = "Accessories", CreatedAt = createdAt },
            };

            var products = new List<Product>()
            {
                new Product { Id = 1, Name = "Laptop", Description = "High performance laptop", Price = 1000, StockQuantity = 10, CategoryId = 1, CreatedAt = createdAt },
                new Product { Id = 2, Name = "Smartphone", Description = "Latest model smartphone", Price = 500, StockQuantity = 20, CategoryId = 3, CreatedAt = createdAt },
                new Product { Id = 3, Name = "Tablet", Description = "Lightweight tablet", Price = 300, StockQuantity = 15, CategoryId = 3, CreatedAt = createdAt },
                new Product { Id = 4, Name = "Headphones", Description = "Noise-cancelling headphones", Price = 200, StockQuantity = 25, CategoryId = 4, CreatedAt = createdAt },
                new Product { Id = 5, Name = "Smartwatch", Description = "Feature-rich smartwatch", Price = 200, StockQuantity = 30, CategoryId = 4, CreatedAt = createdAt },
                new Product { Id = 6, Name = "PC", Description = "High-performance PC", Price = 500, StockQuantity = 15, CategoryId = 2, CreatedAt = createdAt },
            };

            modelBuilder.Entity<Category>().HasData(categories);
            modelBuilder.Entity<Product>().HasData(products);

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.CartItems)
                    .WithOne(ci => ci.Cart)
                    .HasForeignKey(ci => ci.CartId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasOne(ci => ci.Product)
                    .WithMany()
                    .HasForeignKey(ci => ci.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(ci => ci.UnitPrice)
                    .HasColumnType("decimal(8,2)");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.User)
                    .WithMany()
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(o => o.OrderItems)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(o => o.TotalAmount)
                    .HasColumnType("decimal(8,2)");

                entity.Property(o => o.Status)
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(oi => oi.Product)
                    .WithMany()
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(oi => oi.UnitPrice)
                    .HasColumnType("decimal(8,2)");
            });
        }

        public virtual DbSet<Category> Categories => Set<Category>();
        public virtual DbSet<Product> Products => Set<Product>();
        public virtual DbSet<Cart> Carts => Set<Cart>();
        public virtual DbSet<CartItem> CartItems => Set<CartItem>();
        public virtual DbSet<Order> Orders => Set<Order>();
        public virtual DbSet<OrderItem> OrderItems => Set<OrderItem>();
    }
}
