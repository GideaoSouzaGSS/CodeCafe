using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeCafe.ApiService.Features.Follow.Events;
using CodeCafe.Data;

namespace CodeCafe.ApiService.Features.Follow.Commands;
public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand>
{
    private readonly AppDbContext _context;
    private readonly IMediator _mediator;

    public FollowUserCommandHandler(AppDbContext context,IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task Handle(FollowUserCommand command, CancellationToken ct)
    {
        var follower = await _context.UserProfiles
            .Include(u => u.Followings)
            .FirstOrDefaultAsync(u => u.Id == command.FollowerId, ct);
            
        var userToFollow = await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.Id == command.UserToFollowId, ct);

        if (follower == null || userToFollow == null)
            throw new Exception("User not found");

        follower.Follow(userToFollow);
        await _context.SaveChangesAsync(ct);

                // Dispara evento de login
        await _mediator.Publish(new UserFollowedEvent(
            Guid.NewGuid(),
            follower.Id,
            userToFollow.Id,
            DateTime.UtcNow
        ), ct);

    }
}