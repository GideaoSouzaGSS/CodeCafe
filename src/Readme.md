Subir o Redis via docker

docker run --name redis-CodeCafe -p 6379:6379 -d redis 

[docker exec it redis-mod redis-cli](https://github.com/qishibo/AnotherRedisDesktopManager/releases)

Abrir e conectar normalmente, nao precisa alterar nada caso a porta seja default



**Para atualizar o banco de dados
dotnet ef migrations add InitialCreate --project ../CodeCafe.Data --startup-project . --output-dir Persistence/Migrations --context AppDbContext
dotnet ef database update --project ../CodeCafe.Data --startup-project .  --context AppDbContext

dotnet ef migrations add InitialCreateEvents --project ../CodeCafe.Data --startup-project . --output-dir Persistence/Migrations --context EventStoreDbContext
dotnet ef database update --project ../CodeCafe.Data --startup-project .  --context EventStoreDbContext



PARA RODAR NO DOCKER 

docker-compose build
docker-compose up

Acessar aos emails: http://localhost:8025/#
Redis: localhost:6379
Swagger: http://localhost:5000/swagger/index.html


Dados estao inmemory, quando sair do servico, tudo se apaga.