using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using CodeCafe.ApiService.Features.Chat.Commands.MarkMessageAsRead;
using CodeCafe.ApiService.Features.Chat.Commands.SendMessage;
using CodeCafe.Application.Interfaces.Services;

namespace CodeCafe.ApiService.Features.Chat.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService CurrentUserService;

    public ChatHub(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        CurrentUserService = currentUserService;
    }

    public async Task SendMessage(Guid recipientId, string content)
    {
        var senderId = CurrentUserService.ProfileId;
        var messageId = await _mediator.Send(new SendMessageCommand(senderId, recipientId, content)); 
        
        // Notificação opcional via Hub
        await Clients.User(recipientId.ToString())
            .SendAsync("ReceiveMessage", new { messageId, senderId, content });
    }

    public async Task MarkAsRead(Guid messageId)
    {
        var userId = CurrentUserService.ProfileId;
        await _mediator.Send(new MarkMessageAsReadCommand(messageId, userId));
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        await base.OnConnectedAsync();
    }
}