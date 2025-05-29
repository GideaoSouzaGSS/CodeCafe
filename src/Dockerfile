FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
# Removemos a exposição da porta 443 (HTTPS)

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CodeCafe.ApiService/CodeCafe.ApiService.csproj", "CodeCafe.ApiService/"]
COPY ["CodeCafe.Application/CodeCafe.Application.csproj", "CodeCafe.Application/"]
COPY ["CodeCafe.Domain/CodeCafe.Domain.csproj", "CodeCafe.Domain/"]
COPY ["CodeCafe.Data/CodeCafe.Data.csproj", "CodeCafe.Data/"]
COPY ["CodeCafe.Infrastructure/CodeCafe.Infrastructure.csproj", "CodeCafe.Infrastructure/"]
RUN dotnet restore "CodeCafe.ApiService/CodeCafe.ApiService.csproj"
COPY . .
WORKDIR "/src/CodeCafe.ApiService"
RUN dotnet build "CodeCafe.ApiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeCafe.ApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configurar apenas HTTP
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "CodeCafe.ApiService.dll"]