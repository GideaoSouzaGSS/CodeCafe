using CodeCafe.ApiService.Features.Auth.Commands.EmailConfirmation;
using CodeCafe.ApiService.Features.Auth.Commands.Result;
using CodeCafe.Domain.Interfaces;
using CodeCafe.Messaging.Messages; 
using MassTransit;
using MediatR;

namespace CodeCafe.ApiService.Features.Auth.Handlers.EmailConfirmation;

public class SolicitarConfirmacaoEmailHandler : IRequestHandler<SolicitarConfirmacaoEmailCommand, CommandResult>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IUsuarioRepository _usuarioRepository;

    public SolicitarConfirmacaoEmailHandler(IUsuarioRepository usuarioRepository, IPublishEndpoint publishEndpoint)
    {
        _usuarioRepository = usuarioRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CommandResult> Handle(SolicitarConfirmacaoEmailCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);
        if (usuario == null)
            return CommandResult.Failure("Usuário não encontrado");

        if (usuario.EmailConfirmado)
            return CommandResult.Failure("Email já confirmado");

        // Gera um novo código de confirmação
        usuario.GerarCodigoConfirmacaoEmail();
        await _usuarioRepository.AtualizarAsync(usuario);

        await _publishEndpoint.Publish(new ResendEmailConfirmationEvent
        {
            UserId = usuario.UsuarioId,
            Email = usuario.Email,
            Username = usuario.Email, // Ou outro campo apropriado
            ConfirmationToken = usuario.CodigoConfirmacaoEmail ?? Guid.NewGuid().ToString()
        }, cancellationToken);


        return CommandResult.SuccessResult();
    }
}