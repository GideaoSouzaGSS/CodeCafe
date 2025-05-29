using CodeCafe.ApiService.Features.Auth.Commands.EmailConfirmation;
using CodeCafe.ApiService.Features.Auth.Commands.Result;
using CodeCafe.Domain.Interfaces;
using MediatR;

namespace CodeCafe.ApiService.Features.Auth.Handlers.EmailConfirmation;

public class ConfirmarEmailHandler : IRequestHandler<ConfirmarEmailCommand, CommandResult>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public ConfirmarEmailHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<CommandResult> Handle(ConfirmarEmailCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);
        if (usuario == null)
            return CommandResult.Failure("Usuário não encontrado");

        if (usuario.EmailConfirmado)
            // Retornar sucesso com mensagem informativa em vez de erro
            return CommandResult.SuccessResult("Email já confirmado anteriormente", new { AlreadyConfirmed = true });

        if (string.IsNullOrEmpty(usuario.CodigoConfirmacaoEmail))
            return CommandResult.Failure("Não há solicitação de confirmação pendente");

        if (!usuario.ConfirmarEmail(request.Codigo))
            return CommandResult.Failure("Código de confirmação inválido");
            
        await _usuarioRepository.AtualizarAsync(usuario);
        return CommandResult.SuccessResult("Email confirmado com sucesso");
    }
}