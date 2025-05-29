using MediatR;
using StackExchange.Redis;
using System.Text.Json;
using CodeCafe.ApiService.Features.Auth.Projections;

namespace CodeCafe.ApiService.Features.Auth.Queries;

public record GetUsuarioQuery(Guid Id) : IRequest<UsuarioProjection>;

public class GetUsuarioQueryHandler : IRequestHandler<GetUsuarioQuery, UsuarioProjection>
{
    private readonly IConnectionMultiplexer _redis;

    public GetUsuarioQueryHandler(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<UsuarioProjection> Handle(GetUsuarioQuery query, CancellationToken cancellationToken)
    {
        var db = _redis.GetDatabase();
        var key = $"usuario:{query.Id}";
        
        var data = await db.StringGetAsync(key);
        if (data.IsNull)
            return null;

        return JsonSerializer.Deserialize<UsuarioProjection>(data);
    }
}