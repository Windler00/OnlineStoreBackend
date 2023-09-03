namespace OnlineStoreBackend.Dto.Auth
{
    public class ChangePassDto
    {
        public string NewPass { get; set; } = string.Empty;
        public string NewPassRepeat { get; set; } = string.Empty;
    }
}
