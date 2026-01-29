namespace ShopApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty; // 帳號用 Email 比較標準
        public string PasswordHash { get; set; } = string.Empty; // 絕對不能存明碼！

        // 角色關聯 (1=系統管理者, 2=後台管理者, 3=一般使用者)
        public int RoleId { get; set; } = 3;
        public Role? Role { get; set; }

        // 個人資料
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}