using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CodeCafe.ApiService;
using CodeCafe.Data;

namespace CodeCafe.Tests.Common.Fixtures;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Encontrar o descritor do DbContext
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }
            
            // Adicionar DbContext em memória
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });
            
            // Configurar serviços simulados conforme necessário
        });
    }
}