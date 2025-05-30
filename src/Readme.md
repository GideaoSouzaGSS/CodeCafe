# CodeCafe

## Visão Geral

O CodeCafe é uma aplicação .NET moderna que utiliza arquitetura modular, mensageria, autenticação JWT, armazenamento em nuvem e cache distribuído. O projeto é voltado para aprendizado e demonstração de práticas avançadas de desenvolvimento backend.

## Arquitetura de Pastas

```
src/
  CodeCafe.ApiService/         # API principal (ASP.NET Core)
  CodeCafe.AppHost/            # Host de aplicação (background jobs, workers, etc)
  CodeCafe.Application/        # Camada de aplicação (serviços, comandos, handlers)
  CodeCafe.Data/               # Persistência de dados (EF Core, EventStore)
  CodeCafe.Domain/             # Entidades de domínio e regras de negócio
  CodeCafe.EmailWorker/        # Worker para envio de emails (MassTransit/RabbitMQ)
  CodeCafe.Infrastructure/     # Serviços externos (Email, Blob Storage, etc)
  CodeCafe.Messaging/          # Integração com mensageria
  CodeCafe.ServiceDefaults/    # Configurações e padrões de serviço
  CodeCafe.Tests/              # Testes automatizados
  .config/                     # Ferramentas e configurações do dotnet
  coverage-report/             # Relatórios de cobertura de testes
  tools/                       # Scripts e utilitários
  CodeCafe.sln                 # Solution file
  docker-compose.yml           # Orquestração de containers
  Dockerfile                   # Build da API
  deployment.yaml              # Deploy Kubernetes (se aplicável)
  .env                         # Variáveis de ambiente
```

## Principais Tecnologias e Dependências

- **.NET 9.0**
- **ASP.NET Core** (API REST, SignalR)
- **Entity Framework Core** (banco InMemory para dev/test)
- **MassTransit + RabbitMQ** (mensageria)
- **Redis** (cache e projeções)
- **Azure Blob Storage** (armazenamento de arquivos)
- **FluentValidation** (validação)
- **MediatR** (CQRS)
- **Swagger** (documentação da API)
- **xUnit** (testes)
- **Docker** (containerização)

## Como rodar o projeto

### 1. Subir dependências com Docker

```sh
docker-compose build
docker-compose up
```

- Acesse os emails: [http://localhost:8025/#](http://localhost:8025/#)
- Redis: `localhost:6379`
- Swagger: [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)

### 2. Banco de Dados

Os dados estão em memória (InMemory). Ao parar o serviço, tudo é apagado.

#### Para atualizar o banco de dados:

```sh
dotnet ef migrations add InitialCreate --project ../CodeCafe.Data --startup-project . --output-dir Persistence/Migrations --context AppDbContext
dotnet ef database update --project ../CodeCafe.Data --startup-project .  --context AppDbContext

dotnet ef migrations add InitialCreateEvents --project ../CodeCafe.Data --startup-project . --output-dir Persistence/Migrations --context EventStoreDbContext
dotnet ef database update --project ../CodeCafe.Data --startup-project .  --context EventStoreDbContext
```

### 3. Testes e Cobertura

Para rodar os testes e gerar relatório de cobertura:

```sh
dotnet test src/CodeCafe.sln --collect:"XPlat Code Coverage"
```

Para gerar o relatório HTML de cobertura, instale o [ReportGenerator](https://github.com/danielpalme/ReportGenerator) e execute:

```sh
dotnet reportgenerator -reports:src/CodeCafe.Tests/TestResults/coverage.cobertura.xml -targetdir:coverage-report
```

O relatório estará disponível na pasta `coverage-report`.

---

## Observações

- O projeto utiliza autenticação JWT e suporte a autenticação de dois fatores.
- O envio de emails é feito via worker dedicado, consumindo eventos do RabbitMQ.
- O armazenamento de arquivos é feito em containers do Azure Blob Storage.
- O cache e as projeções de usuários são mantidos no Redis.

---

Para dúvidas ou contribuições, consulte os arquivos de cada módulo ou abra uma issue.