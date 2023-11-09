namespace OnlineStoreBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Role { get; set; } = "User";
        public string PasswordHash { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public Order Order { get; set; }
        public List<BasketItem> BasketList { get; set; }
    }
}