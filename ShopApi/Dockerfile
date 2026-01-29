# 1. 建置階段 (Build Stage) - 使用 SDK 映像檔來編譯程式碼
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 先複製專案檔就好 (利用 Docker 快取機制，加速建置)
COPY ["ShopApi.csproj", "./"]
RUN dotnet restore "ShopApi.csproj"

# 再複製剩下的所有程式碼
COPY . .

# 開始編譯並發布 (Publish)
RUN dotnet publish "ShopApi.csproj" -c Release -o /app/publish

# 2. 執行階段 (Runtime Stage) - 使用輕量級 ASP.NET Core 映像檔來執行
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# 設定 Port (Render 預設喜歡 8080)
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

# 啟動程式
ENTRYPOINT ["dotnet", "ShopApi.dll"]