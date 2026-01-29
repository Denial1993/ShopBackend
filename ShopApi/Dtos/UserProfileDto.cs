namespace ShopApi.Dtos
{
    public class UserProfileDto
    {
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
