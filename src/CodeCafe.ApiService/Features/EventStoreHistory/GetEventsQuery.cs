using MediatR;
using CodeCafe.Data.Store;

namespace CodeCafe.ApiService.Features.EventStoreHistory;
public class GetEventsQuery : IRequest<List<EventData>>
{
}