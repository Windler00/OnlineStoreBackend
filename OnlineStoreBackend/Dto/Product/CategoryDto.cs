namespace OnlineStoreBackend.Dto.Product
{
    public class CategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public int ParentId;
    }
}