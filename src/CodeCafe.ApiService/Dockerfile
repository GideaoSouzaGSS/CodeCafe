# Usa a imagem base do ASP.NET Core 9.0 apenas com o runtime para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# Define o diretório de trabalho como /app na imagem final
WORKDIR /app
# Expõe a porta 80 (HTTP) do container
EXPOSE 80
# Removemos a exposição da porta 443 (HTTPS)

# Usa a imagem do SDK do .NET 9.0 para compilar e publicar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Define o diretório de trabalho como /src durante o build
WORKDIR /src
# Copia o arquivo de projeto principal para dentro do container
COPY ["CodeCafe.ApiService/CodeCafe.ApiService.csproj", "CodeCafe.ApiService/"]
# Copia os arquivos de projeto das dependências para dentro do container
COPY ["CodeCafe.Application/CodeCafe.Application.csproj", "CodeCafe.Application/"]
COPY ["CodeCafe.Domain/CodeCafe.Domain.csproj", "CodeCafe.Domain/"]
COPY ["CodeCafe.Data/CodeCafe.Data.csproj", "CodeCafe.Data/"]
COPY ["CodeCafe.Infrastructure/CodeCafe.Infrastructure.csproj", "CodeCafe.Infrastructure/"]
# Restaura as dependências do projeto principal
RUN dotnet restore "CodeCafe.ApiService/CodeCafe.ApiService.csproj"
# Copia todo o restante do código-fonte para dentro do container
COPY . .
# Define o diretório de trabalho como a pasta do projeto principal
WORKDIR "/src/CodeCafe.ApiService"
# Compila o projeto em modo Release e coloca o resultado em /app/build
RUN dotnet build "CodeCafe.ApiService.csproj" -c Release -o /app/build

# Cria uma nova etapa chamada publish baseada na etapa build
FROM build AS publish
# Publica a aplicação em modo Release para a pasta /app/publish, sem gerar host nativo
RUN dotnet publish "CodeCafe.ApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Cria a imagem final baseada na imagem base (runtime)
FROM base AS final
# Define o diretório de trabalho como /app na imagem final
WORKDIR /app
# Copia os arquivos publicados da etapa publish para a imagem final
COPY --from=publish /app/publish .

# Configura a aplicação para escutar apenas HTTP na porta 80
ENV ASPNETCORE_URLS=http://+:80
# Define o ambiente como Development
ENV ASPNETCORE_ENVIRONMENT=Development

# Define o comando de inicialização do container
ENTRYPOINT ["dotnet", "CodeCafe.ApiService.dll"]