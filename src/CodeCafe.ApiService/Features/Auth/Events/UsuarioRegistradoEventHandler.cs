using MediatR;
using StackExchange.Redis;
using System.Text.Json;
using CodeCafe.ApiService.Features.Auth.Projections;
using CodeCafe.Data.Store;
using CodeCafe.ApiService.Features.Usuarios.Models;

namespace CodeCafe.ApiService.Features.Auth.Events;

public class UsuarioRegistradoEventHandler : INotificationHandler<UsuarioRegistradoEvent>
{
    private readonly IEventStore _eventStore;
    private readonly IConnectionMultiplexer _redis;

    public UsuarioRegistradoEventHandler(
        IEventStore eventStore,
        IConnectionMultiplexer redis)
    {
        _eventStore = eventStore;
        _redis = redis;
    }

    public async Task Handle(UsuarioRegistradoEvent @event, CancellationToken cancellationToken)
    {
        // Salva o evento no EventStore
        await _eventStore.SaveEventAsync(@event);

        // Cria a projeção
        var projection = new UsuarioProjection
        {
            Id = @event.UsuarioId,
            Nome = @event.Nome,
            Email = @event.Email,
            DataNascimento = @event.DataNascimento,
            DataCriacao = @event.DataRegistro
        };

        var db = _redis.GetDatabase();
        
        // Salva a projeção no Redis
        var key = $"usuario:{projection.Id}";
        await db.StringSetAsync(
            key, 
            JsonSerializer.Serialize(projection),
            TimeSpan.FromHours(24) // TTL de 24 horas
        );

        // Índice secundário para busca por email
        await db.StringSetAsync($"usuario:email:{projection.Email}", projection.Id.ToString());
    }
}