using CodeCafe.ApiService.Features.Auth.Commands.Result;
using CodeCafe.ApiService.Features.Auth.Commands.TwoFactor;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Domain.Interfaces;
using MediatR;

namespace CodeCafe.ApiService.Features.Auth.Handlers.TwoFactor;

public class DesabilitarTwoFactorCommandHandler : IRequestHandler<DesabilitarTwoFactorCommand, CommandResult>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;

    public DesabilitarTwoFactorCommandHandler(
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService)
    {
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CommandResult> Handle(DesabilitarTwoFactorCommand request, CancellationToken cancellationToken)
    {
        // Verificar se o usuário está autenticado
        if (!_currentUserService.IsAuthenticated)
            return CommandResult.Failure("Usuário não autenticado");

        // Obter o usuário atual
        var usuario = await _usuarioRepository.ObterPorIdAsync(_currentUserService.UserId);
        if (usuario == null)
            return CommandResult.Failure("Usuário não encontrado");

        // Verificar se o 2FA está habilitado
        if (!usuario.TwoFactorHabilitado)
            return CommandResult.Failure("Autenticação de dois fatores não está habilitada");

        // Verificar a senha para confirmar identidade
        if (!usuario.VerificarSenha(request.Senha))
            return CommandResult.Failure("Senha incorreta");

        // Desabilitar 2FA
        usuario.DesabilitarTwoFactor();
        
        // Salvar alterações
        await _usuarioRepository.AtualizarAsync(usuario);

        return CommandResult.SuccessResult();
    }
}