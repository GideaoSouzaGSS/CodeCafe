using CodeCafe.Application.Interfaces.Services;
using CodeCafe.EmailWorker.Consumers;
using CodeCafe.Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        // Adiciona o appsettings.json da API
        string apiConfigPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, 
                                          "CodeCafe.Api", "appsettings.json");
        
        if (File.Exists(apiConfigPath))
        {
            config.AddJsonFile(apiConfigPath, optional: true, reloadOnChange: true);
        }
        
        // Adiciona o appsettings específico do ambiente da API
        string apiEnvironmentConfig = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                                                "CodeCafe.Api", $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json");
        
        if (File.Exists(apiEnvironmentConfig))
        {
            config.AddJsonFile(apiEnvironmentConfig, optional: true, reloadOnChange: true);
        }
    })
    .ConfigureServices((hostContext, services) =>
    {
        // Configuração do EmailService
        services.Configure<EmailSettings>(hostContext.Configuration.GetSection("Email"));
        services.AddTransient<IEmailService, EmailService>();
        
        // Configuração do MassTransit
        services.AddMassTransit(busConfig =>
        {
            busConfig.AddConsumer<UserRegisteredConsumer>();
            busConfig.AddConsumer<PasswordResetRequestedConsumer>();
            busConfig.AddConsumer<ResenEmailConfirmationConsumer>();
            // busConfig.AddConsumer<ProfileUpdatedConsumer>();

            busConfig.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

await host.RunAsync();
