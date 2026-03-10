using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Data;
using Microsoft.OpenApi.Models;
using ShopApi.Services;
using ShopApi.Validators;
using FluentValidation.AspNetCore;
using FluentValidation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 找到原本的 UseSqlServer，改成 UseNpgsql
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// --- ⬇️ 新增這段 (設定 JWT 驗證邏輯) ⬇️ ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("Jwt:Key").Value!)),
            ValidateIssuer = false, // 暫時不檢查發發行者 (簡化練習)
            ValidateAudience = false // 暫時不檢查接收者
        };
    });

builder.Services.AddEndpointsApiExplorer();
// Supabase Configuration
builder.Services.AddScoped<Supabase.Client>(_ =>
    new Supabase.Client(
        builder.Configuration["Supabase:Url"]!,
        builder.Configuration["Supabase:Key"],
        new Supabase.SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        }));

builder.Services.AddSwaggerGen(options =>
{
    // 1. 定義安全機制 (告訴 Swagger 我們要用 JWT Bearer)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", // Header 的名字
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "請在下方輸入：Bearer {你的Token} (注意 Bearer 後面有空白)"
    });

    // 2. 套用安全機制 (告訴 Swagger 所有的 API 都受這個保護)
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // --- ⬇️ 新增這段 (載入 XML 註解) ⬇️ ---
    // 1. 取得 XML 檔案名稱 (通常是 專案名稱.xml)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // 2. 組合出完整路徑
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // 3. 告訴 Swagger 使用它
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ECPayService>();

// --- ⬇️ 新增這段 (註冊 Semantic Kernel 與 Gemini 服務) ⬇️ ---
var aiKey = builder.Configuration["AI:ApiKey"];
if (!string.IsNullOrEmpty(aiKey) && aiKey != "YOUR_GEMINI_API_KEY_HERE")
{
    builder.Services.AddKernel()
        .AddGoogleAIGeminiChatCompletion(
            modelId: builder.Configuration["AI:ModelId"]!,
            apiKey: aiKey);
}
else
{
    // 若沒有 Key, 依舊註冊一個空的 Kernel 避免 injection 失敗，但會在 Controller catch 處理
    builder.Services.AddKernel();
}
// --- ⬆️ 新增這段 ⬆️ ---
// 2. 註冊 FluentValidation
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<UserDtoValidator>(); // 自動掃描所有 Validator

builder.Services.AddControllers();

// 開放 CORS (允許跨網域請求)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowShopFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",                       // 1. 給你本機開發用 (Vue)
            "https://shop-frontend-z8a0.onrender.com"      // 2. 給正式環境用 (Render)
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });

    options.AddPolicy("AllowRenderFrontend",
        policy =>
        {
            // 👇 這是你截圖裡的前端網址 (注意：後面不要有斜線 /)
            policy.WithOrigins("https://shop-frontend-z8a0.onrender.com")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// 這是 .NET 8 取得 Service 的標準寫法
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ShopDbContext>();
    // 呼叫我們剛剛寫的補貨機
    // DbInitializer.Initialize(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// app.UseHttpsRedirection(); //這行會幫我倒回去 https ，先註解掉


app.UseCors("AllowShopFrontend");

// --- ⬇️ 新增這段 (開啟驗證與授權 Middleware) ⬇️ ---
app.UseAuthentication(); // 先檢查你是誰 (查票)
app.UseAuthorization();  // 再檢查你能做什麼 (查權限)
// --- ⬆️ 新增這段 (注意順序！要放在 MapControllers 之前) ⬆️ ---

app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
