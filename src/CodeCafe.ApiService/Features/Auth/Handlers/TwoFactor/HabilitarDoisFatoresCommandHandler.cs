using CodeCafe.ApiService.Features.Auth.Commands.Result;
using CodeCafe.ApiService.Features.Auth.Commands.TwoFactor;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Domain.Interfaces;
using MediatR;

namespace CodeCafe.ApiService.Features.Auth.Handlers.TwoFactor;

public class HabilitarTwoFactorCommandHandler : IRequestHandler<HabilitarTwoFactorCommand, CommandResult>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITwoFactorService _twoFactorService;

    public HabilitarTwoFactorCommandHandler(
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService,
        ITwoFactorService twoFactorService)
    {
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
        _twoFactorService = twoFactorService;
    }

    public async Task<CommandResult> Handle(HabilitarTwoFactorCommand request, CancellationToken cancellationToken)
    {
        // Obter o usuário autenticado atual
        var usuarioId = _currentUserService.UserId; 
        if (usuarioId == null)
            return CommandResult.Failure("Usuário não autenticado");

        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            return CommandResult.Failure("Usuário não encontrado");

        // Verifica se o email está confirmado
        if (!usuario.EmailConfirmado)
            return CommandResult.Failure("É necessário confirmar o email antes de habilitar 2FA");

        // Se o 2FA já está habilitado
        if (usuario.TwoFactorHabilitado)
            return CommandResult.Failure("Autenticação de dois fatores já está habilitada");

        // Gerar uma chave secreta
        var chaveSecreta = _twoFactorService.GerarChaveSecreta();

        // Validar o código fornecido pelo usuário
        var codigoValido = _twoFactorService.ValidarCodigo(chaveSecreta, request.Codigo);
        if (!codigoValido)
            return CommandResult.Failure("Código inválido");

        // Habilitar 2FA para o usuário
        usuario.HabilitarTwoFactor(chaveSecreta);
        await _usuarioRepository.AtualizarAsync(usuario);

        // Gerar QR code URL
        var qrCodeUrl = _twoFactorService.GerarQrCodeUrl(usuario.Email, chaveSecreta);

        return CommandResult.SuccessResult(data: new
        {
            QrCodeUrl = qrCodeUrl,
            TokensRecuperacao = usuario.TokensRecuperacaoTwoFactor
        });
    }
}