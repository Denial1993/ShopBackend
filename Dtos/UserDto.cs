using System.ComponentModel.DataAnnotations;
namespace ShopApi.Dtos
{
    /// <summary>
    /// 使用者註冊與登入用的資料
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 使用者的電子信箱(必填)
        /// </summary>
        /// <example>daniel@test.com</example>
        [Required]
        public required string Email { get; set; } 
        /// <summary>
        /// 密碼 (至少6碼，需包含大小寫英文與數字)
        /// </summary>
        ///<example>P@ssw0rd123</example>
        [Required]
        public required string Password { get; set; } 
    }
}