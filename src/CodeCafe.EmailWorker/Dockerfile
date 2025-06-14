# Usa a imagem base do ASP.NET Core 9.0 apenas com o runtime para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# Define o diretório de trabalho como /app na imagem final
WORKDIR /app

# Usa a imagem do SDK do .NET 9.0 para compilar e publicar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Define o diretório de trabalho como /src durante o build
WORKDIR /src
# Copia o arquivo de projeto do EmailWorker para dentro do container
COPY ["CodeCafe.EmailWorker/CodeCafe.EmailWorker.csproj", "CodeCafe.EmailWorker/"]
# Copia o arquivo de projeto do Messaging (dependência) para dentro do container
COPY ["CodeCafe.Messaging/CodeCafe.Messaging.csproj", "CodeCafe.Messaging/"]
# Copia o arquivo de projeto do Infrastructure (dependência) para dentro do container
COPY ["CodeCafe.Infrastructure/CodeCafe.Infrastructure.csproj", "CodeCafe.Infrastructure/"]
# Restaura as dependências do projeto principal
RUN dotnet restore "CodeCafe.EmailWorker/CodeCafe.EmailWorker.csproj"
# Copia todo o restante do código-fonte para dentro do container
COPY . .
# Define o diretório de trabalho como a pasta do projeto principal
WORKDIR "/src/CodeCafe.EmailWorker"
# Compila o projeto em modo Release e coloca o resultado em /app/build
RUN dotnet build "CodeCafe.EmailWorker.csproj" -c Release -o /app/build

# Cria uma nova etapa chamada publish baseada na etapa build
FROM build AS publish
# Publica a aplicação em modo Release para a pasta /app/publish, sem gerar host nativo
RUN dotnet publish "CodeCafe.EmailWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Cria a imagem final baseada na imagem base (runtime)
FROM base AS final
# Define o diretório de trabalho como /app na imagem final
WORKDIR /app
# Copia os arquivos publicados da etapa publish para a imagem final
COPY --from=publish /app/publish .
# Define o comando de inicialização do container
ENTRYPOINT ["dotnet", "CodeCafe.EmailWorker.dll"]