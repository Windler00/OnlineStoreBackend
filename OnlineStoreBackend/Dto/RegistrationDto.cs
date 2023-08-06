namespace OnlineStoreBackend.Dto
{
    public class RegistrationDto
    {
        public string Email { get; set; } = string.Empty;
        public string EmailConfirmation { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordConfirmation { get; set;} = string.Empty;
    }
}
