using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        public static string ConnectionString = string.Empty;

        public DataContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasMany(u => u.BasketList)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<BasketItem>()
             .HasOne(b => b.Product)
             .WithOne(p => p.BasketItem);

            modelBuilder.Entity<Product>()
             .HasMany(p => p.Categories)
             .WithMany(c => c.Products)
             .UsingEntity(j => j.ToTable("ProductCategory"));

            string passwordHash = BCrypt.Net.BCrypt.HashPassword("AdminRoot123");
            User admin = new User();
            admin.Id = 1;
            admin.Name = "admin";
            admin.Email = "admin@email.com";
            admin.Role = "Admin";
            admin.PasswordHash = passwordHash;
            modelBuilder.Entity<User>().HasData(admin);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ConnectionString);
        }
    }
}
