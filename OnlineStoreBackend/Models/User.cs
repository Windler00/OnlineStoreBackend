﻿namespace OnlineStoreBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Balance { get; set; }
        public string Role { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

    }
}