﻿namespace OnlineStoreBackend.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ParentId { get; set; }
        public List<Product> Products { get; set; }
    }
}
