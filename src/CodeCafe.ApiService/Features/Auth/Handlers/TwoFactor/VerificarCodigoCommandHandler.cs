using CodeCafe.ApiService.Features.Auth.Commands.Result;
using CodeCafe.ApiService.Features.Auth.Commands.TwoFactor;
using CodeCafe.ApiService.Features.Auth.Services;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Domain.Interfaces;
using MediatR;

namespace CodeCafe.ApiService.Features.Auth.Handlers.TwoFactor;
public class VerificarCodigoCommandHandler : IRequestHandler<VerificarCodigoTwoFactorCommand, CommandResult>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITwoFactorService _twoFactorService;
    private readonly IJwtTokenService _jwtTokenService;

    public VerificarCodigoCommandHandler(
        IUsuarioRepository usuarioRepository,
        ITwoFactorService twoFactorService,
        IJwtTokenService jwtTokenService)
    {
        _usuarioRepository = usuarioRepository;
        _twoFactorService = twoFactorService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<CommandResult> Handle(VerificarCodigoTwoFactorCommand request, CancellationToken cancellationToken)
    {
        // Validação básica
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Codigo))
            return CommandResult.Failure("Email e código são obrigatórios");

        // Buscar usuário pelo email
        var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);
        if (usuario == null)
            return CommandResult.Failure("Usuário não encontrado");

        // Verificar se 2FA está habilitado
        if (!usuario.TwoFactorHabilitado)
            return CommandResult.Failure("Autenticação de dois fatores não está habilitada para este usuário");

        bool codigoValido = false;
        
        // Verificar se é um token de recuperação
        if (usuario.TokensRecuperacaoTwoFactor.Contains(request.Codigo))
        {
            // Remover o token de recuperação usado
            usuario.TokensRecuperacaoTwoFactor.Remove(request.Codigo);
            await _usuarioRepository.AtualizarAsync(usuario);
            codigoValido = true;
        }
        else
        {
            // Verificar código do app autenticador
            codigoValido = _twoFactorService.ValidarCodigo(
                usuario.ChaveAutenticacaoTwoFactor, 
                request.Codigo);
        }

        if (!codigoValido)
            return CommandResult.Failure("Código inválido");

        // Gerar token JWT para autenticação
        var token = _jwtTokenService.GenerateToken(usuario);
        
        return CommandResult.SuccessResult(data: new { Token = token });
    }
}