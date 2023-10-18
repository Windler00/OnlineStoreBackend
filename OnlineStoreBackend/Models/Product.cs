namespace OnlineStoreBackend.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageName { get; set; } = string.Empty;
        public byte[] ImageData { get; set; } = new byte[0];
        public List<Category> Categories { get; set; }
    }
}