using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeCafe.Data;

namespace CodeCafe.ApiService.Features.Follow.Commands;    

public class UnfollowUserCommandHandler : IRequestHandler<UnfollowUserCommand>
{
    private readonly AppDbContext _context;

    public UnfollowUserCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UnfollowUserCommand command, CancellationToken ct)
    {
        // REVISAR O UNFOLLOW
        var follower = await _context.UserProfiles
            .Include(u => u.Followings)
            .FirstOrDefaultAsync(u => u.Id == command.FollowerId, ct);
            
        var userToFollow = await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.Id == command.UserToUnfollowId, ct);

        if (follower == null || userToFollow == null)
            throw new Exception("User not found");

        follower.Unfollow(userToFollow);
        await _context.SaveChangesAsync(ct);
    }
}