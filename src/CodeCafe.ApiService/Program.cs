using System.Text;
using Azure.Storage.Blobs;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CodeCafe.ApiService.Behaviors;
using CodeCafe.ApiService.Features.Albums;
using CodeCafe.ApiService.Features.Auth.Endpoints;
using CodeCafe.ApiService.Features.Auth.Services;
using CodeCafe.ApiService.Features.Chat.Commands.MarkMessageAsRead;
using CodeCafe.ApiService.Features.Chat.Commands.SendMessage;
using CodeCafe.ApiService.Features.Chat.Hubs;
using CodeCafe.ApiService.Features.Chat.Queries.GetConversation;
using CodeCafe.ApiService.Features.EventStoreHistory;
using CodeCafe.ApiService.Features.Follow;
using CodeCafe.ApiService.Features.Upload.Endpoints;
using CodeCafe.ApiService.Middleware;
using CodeCafe.Data;
using CodeCafe.Data.Repositories;
using CodeCafe.Domain.Interfaces;
using CodeCafe.Data.Store;
using CodeCafe.Infrastructure.Configuration;
using CodeCafe.Infrastructure.Services;
using Microsoft.Extensions.Options;
using CodeCafe.Application.Interfaces.Services;
using StackExchange.Redis;
using CodeCafe.ApiService.Features.Auth;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Adicione no início do método Main
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5507", "http://127.0.0.1:5500", 
         "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Isso é crucial para SignalR
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

// Configuração básica
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();

// Configuração do JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddAntiforgery(options => 
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
});
// Repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IEventStore, EventStoreRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();

// Adicione ao registro de serviços
builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();
 
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("AppDbContext"));

builder.Services.AddDbContext<EventStoreDbContext>(options =>
    options.UseInMemoryDatabase("EventStoreDbContext"));

// MediatR e Validação
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Serviços de aplicação
// No arquivo de configuração de serviços (Program.cs ou Startup.cs)
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IAlbumsRepository, AlbumsRepository>();

builder.Services.Configure<BlobStorageOptions>(
    builder.Configuration.GetSection(BlobStorageOptions.ConfigurationSection));

builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<BlobStorageOptions>>().Value;
    return new BlobServiceClient(options.ConnectionString);
});

builder.Services.AddScoped<IBlobService, AzureBlobService>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<BlobServiceClient>(_ =>
        new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage")));

    builder.Services.AddScoped<IPublicImageBlobService, PublicImageBlobService>();
    builder.Services.AddScoped<IProfileImageBlobService, ProfileImageBlobService>();
    builder.Services.AddScoped<IPrivateImageBlobService, PrivateImageBlobService>();
}
else
{
    builder.Services.AddSingleton<BlobServiceClient>(_ =>
        new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage")));

    builder.Services.AddScoped<IPublicImageBlobService, PublicImageBlobService>();
    builder.Services.AddScoped<IProfileImageBlobService, ProfileImageBlobService>();
    builder.Services.AddScoped<IPrivateImageBlobService, PrivateImageBlobService>();
}

// Configuração do Redis
// Substituir a configuração atual do Redis por esta
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis");
    var options = ConfigurationOptions.Parse(configuration);
    options.AbortOnConnectFail = false;
    options.SyncTimeout = 30000;
    options.ConnectTimeout = 30000;
    // Remova a linha com ConnectRetryPolicy pois não existe na sua versão
    options.ReconnectRetryPolicy = new ExponentialRetry(5000);
    options.Ssl = false; // Forçar desativação de SSL

    var connection = ConnectionMultiplexer.Connect(options);
    connection.ConnectionFailed += (sender, args) => 
    {
        Console.WriteLine($"Conexão Redis falhou: {args.Exception?.Message}");
    };
    connection.ConnectionRestored += (sender, args) => 
    {
        Console.WriteLine($"Conexão Redis restaurada: {args.EndPoint}");
    };
    
    return connection;
});

// Configuração do Redis Distributed Cache também com SSL desativado
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = builder.Configuration.GetValue<string>("Redis:InstanceName");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(busConfig =>
{
    busConfig.UsingRabbitMq((context, cfg) =>
    {
        // Configuração do RabbitMQ
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Configuração das filas
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Garantir que os contêineres existam
using (var scope = app.Services.CreateScope())
{
    var blobService = scope.ServiceProvider.GetRequiredService<IBlobService>();
    await blobService.CreateContainersAsync();
}

app.UseCors("SignalRPolicy");

// Middlewares
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication(); // Adicione esta linha
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoints
app.MapDefaultEndpoints();
app.MapAuthEndpoints();
app.MapEmailConfirmationEndpoints();
app.MapTwoFactorEndpoints();
app.MapSendMessageEndpoint();
app.MapMarkMessageAsReadEndpoint();
app.MapGetConversationEndpoint();
app.MapProfileEndpoints();

app.UseAntiforgery();
app.MapImageEndpoints(); 
app.MapAlbumsEndpoints();
app.EventMapEndpoints();

app.MapHub<ChatHub>("/MeuChat");

app.Run();
