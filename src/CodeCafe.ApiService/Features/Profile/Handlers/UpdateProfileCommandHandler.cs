using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using CodeCafe.ApiService.Features.Profile.Events;
using CodeCafe.Data;
using CodeCafe.Domain.Entities;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Domain.Interfaces;

namespace CodeCafe.ApiService.Features.Profile.Commands;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;
    private readonly IProfileRepository _repository;

    public UpdateProfileCommandHandler(
        IProfileRepository repository,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (_currentUserService.ProfileId == Guid.Empty)
        {
            await _repository.CreateProfileForUserAsync(userId, cancellationToken);
        }
        else
        {
            var user = await _repository.GetUserProfileAsync(_currentUserService.ProfileId, cancellationToken);
            if (user == null)
                throw new Exception("User profile not found");
            // Atualizar perfil existente
            user.UpdateProfile(
                request.Username,
                request.DisplayName,
                request.Bio,
                request.AcceptFollow,
                request.AcceptDirectMessage
            );

            await _repository.UpdateAsync(user, cancellationToken);
            
        }       

        // Publicar evento
        var profileUpdatedEvent = new ProfileUpdatedEvent(
            userId,
            request.Username,
            request.DisplayName,
            request.Bio,
            request.AcceptFollow,
            request.AcceptDirectMessage
        );

        await _mediator.Publish(profileUpdatedEvent, cancellationToken);

        return true;
    }
}