using MediatR;
using CodeCafe.Data.Store;

namespace CodeCafe.ApiService.Features.EventStoreHistory;

public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, List<EventData>> 
{
    private readonly IEventStore _repository;

    public GetEventsQueryHandler(IEventStore repository)
    {
        _repository = repository;
    }

    public async Task<List<EventData>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllEventsAsync();
    }
}