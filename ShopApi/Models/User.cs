namespace ShopApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty; // 帳號用 Email 比較標準
        public string PasswordHash { get; set; } = string.Empty; // 絕對不能存明碼！

        // 角色 (例如: "Admin", "User")，之後可以做權限管理
        public string Role { get; set; } = "User"; 
    }
}