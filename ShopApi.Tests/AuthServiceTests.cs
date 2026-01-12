using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq; // 模擬工具
using ShopApi.Data;
using ShopApi.Dtos;
using ShopApi.Services;
using Xunit; // 測試框架

namespace ShopApi.Tests
{
    public class AuthServiceTests
    {
        // 這是我們要測試的主角
        private readonly AuthService _authService;
        private readonly ShopDbContext _context;

        public AuthServiceTests()
        {
            // 1. 準備一個「記憶體資料庫」 (這是假的，跑完就消失，速度超快)
            var options = new DbContextOptionsBuilder<ShopDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 每次都用隨機名稱，確保乾淨
                .Options;

            _context = new ShopDbContext(options);

            // 2. 模擬 IConfiguration (因為我們只需要 Jwt:Key，隨便給一個假的就好)
            var mockConfig = new Mock<IConfiguration>();
            // 當 AuthService 跟它要 "Jwt:Key" 的時候，它會回傳下面這串
            mockConfig.Setup(c => c.GetSection("Jwt:Key").Value).Returns("ThisIsAFakeKeyForTestingOnly12345");

            // 3. 把假的 DB 和假的 Config 塞給 AuthService
            _authService = new AuthService(_context, mockConfig.Object);
        }

        [Fact] // 代表這是一個測試案例
        public async Task Register_ShouldReturnSuccess_WhenEmailIsNew()
        {
            // Arrange (準備資料)
            var dto = new UserDto 
            { 
                Email = "newuser@test.com", 
                Password = "Password123" 
            };

            // Act (執行動作)
            var result = await _authService.RegisterAsync(dto);

            // Assert (驗證結果)
            // 1. 預期回傳字串要是 "註冊成功"
            Assert.Equal("註冊成功", result);
            
            // 2. 驗證資料庫裡是不是真的多了一筆資料？
            var userCount = await _context.Users.CountAsync();
            Assert.Equal(1, userCount);
        }

        [Fact]
        public async Task Register_ShouldFail_WhenEmailExists()
        {
            // Arrange
            // 先偷塞一個舊使用者進去
            _context.Users.Add(new ShopApi.Models.User { Email = "old@test.com", PasswordHash = "xxx" });
            await _context.SaveChangesAsync();

            var dto = new UserDto { Email = "old@test.com", Password = "Password123" };

            // Act
            var result = await _authService.RegisterAsync(dto);

            // Assert
            Assert.Equal("Email 已經被註冊過了", result);
        }
    }
}