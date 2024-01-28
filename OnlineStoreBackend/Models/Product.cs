namespace OnlineStoreBackend.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Quantity { get; set; }
        public List<User> Owners { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<Category> Categories { get; set; }
        public BasketItem BasketItem { get; set; }
        public OrderItem OrderItem { get; set; }
    }
}