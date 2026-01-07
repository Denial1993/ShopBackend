# 1. 建置環境 (Build Stage)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 先複製 csproj 並還原套件 (利用 Docker Cache 機制加速)
COPY ["ShopApi.csproj", "./"]
RUN dotnet restore "ShopApi.csproj"

# 複製其餘程式碼並編譯
COPY . .
RUN dotnet publish "ShopApi.csproj" -c Release -o /app/publish

# 2. 執行環境 (Runtime Stage) - 這裡只會有最小的執行檔
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ShopApi.dll"]