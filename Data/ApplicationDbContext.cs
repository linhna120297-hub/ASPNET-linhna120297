using ElectronicsStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Category configuration
            builder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Product configuration
            builder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Cart configuration
            builder.Entity<Cart>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithOne(p => p.Cart)
                    .HasForeignKey<Cart>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CartItem configuration
            builder.Entity<CartItem>(entity =>
            {
                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CartId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Order configuration
            builder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItem configuration
            builder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed data
            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            var baseDate = new DateTime(2024, 1, 1);
            
            // Seed Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Vi xử lý", Slug = "vi-xu-ly", Description = "CPU, Processor", ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate },
                new Category { Id = 2, Name = "Bo mạch chủ", Slug = "bo-mach-chu", Description = "Mainboard, Motherboard", ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate },
                new Category { Id = 3, Name = "RAM", Slug = "ram", Description = "Bộ nhớ trong", ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate },
                new Category { Id = 4, Name = "Ổ cứng", Slug = "o-cung", Description = "HDD, SSD", ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate },
                new Category { Id = 5, Name = "Card đồ họa", Slug = "card-do-hoa", Description = "VGA, Graphics Card", ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate },
                new Category { Id = 6, Name = "Nguồn máy tính", Slug = "nguon-may-tinh", Description = "PSU, Power Supply", ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate }
            );

            // Seed Products
            builder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Intel Core i5-12400F", Slug = "intel-core-i5-12400f", Description = "Vi xử lý Intel Core i5 thế hệ 12", Price = 4500000, CategoryId = 1, StockQuantity = 50, IsFeatured = true, ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate },
                new Product { Id = 2, Name = "AMD Ryzen 5 5600X", Slug = "amd-ryzen-5-5600x", Description = "Vi xử lý AMD Ryzen 5 series 5000", Price = 5200000, CategoryId = 1, StockQuantity = 30, IsFeatured = true, ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate },
                new Product { Id = 3, Name = "ASUS ROG STRIX B550-F", Slug = "asus-rog-strix-b550-f", Description = "Bo mạch chủ ASUS ROG STRIX B550-F Gaming", Price = 4800000, CategoryId = 2, StockQuantity = 25, IsFeatured = true, ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate },
                new Product { Id = 4, Name = "Corsair Vengeance LPX 16GB", Slug = "corsair-vengeance-lpx-16gb", Description = "RAM DDR4 16GB 3200MHz", Price = 1800000, CategoryId = 3, StockQuantity = 100, IsFeatured = true, ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate },
                new Product { Id = 5, Name = "Samsung 980 PRO 1TB", Slug = "samsung-980-pro-1tb", Description = "SSD NVMe M.2 1TB", Price = 3200000, CategoryId = 4, StockQuantity = 40, IsFeatured = true, ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate },
                new Product { Id = 6, Name = "NVIDIA RTX 4060 Ti", Slug = "nvidia-rtx-4060-ti", Description = "Card đồ họa NVIDIA GeForce RTX 4060 Ti", Price = 12500000, CategoryId = 5, StockQuantity = 15, IsFeatured = true, ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate },
                new Product { Id = 7, Name = "Corsair RM750x", Slug = "corsair-rm750x", Description = "Nguồn máy tính 750W 80+ Gold", Price = 2800000, CategoryId = 6, StockQuantity = 35, IsFeatured = false, ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate },
                new Product { Id = 8, Name = "WD Blue 2TB", Slug = "wd-blue-2tb", Description = "Ổ cứng HDD 2TB 7200RPM", Price = 1500000, CategoryId = 4, StockQuantity = 60, IsFeatured = false, ImageUrl = "https://images.pexels.com/photos/2582932/pexels-photo-2582932.jpeg", CreatedAt = baseDate, UpdatedAt = baseDate }
            );
        }
    }
}