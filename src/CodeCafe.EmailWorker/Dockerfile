FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CodeCafe.EmailWorker/CodeCafe.EmailWorker.csproj", "CodeCafe.EmailWorker/"]
COPY ["CodeCafe.Messaging/CodeCafe.Messaging.csproj", "CodeCafe.Messaging/"]
COPY ["CodeCafe.Infrastructure/CodeCafe.Infrastructure.csproj", "CodeCafe.Infrastructure/"]
RUN dotnet restore "CodeCafe.EmailWorker/CodeCafe.EmailWorker.csproj"
COPY . .
WORKDIR "/src/CodeCafe.EmailWorker"
RUN dotnet build "CodeCafe.EmailWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeCafe.EmailWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeCafe.EmailWorker.dll"]