using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public static string ConnectionString = string.Empty;

        public DataContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
