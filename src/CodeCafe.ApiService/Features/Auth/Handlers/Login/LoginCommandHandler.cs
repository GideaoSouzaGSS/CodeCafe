using MediatR;
using CodeCafe.ApiService.Features.Auth.Services;
using CodeCafe.ApiService.Features.Usuarios.Models;
using System.Security;
using CodeCafe.Domain.Interfaces;
using CodeCafe.ApiService.Features.Auth.DTOs;
using CodeCafe.ApiService.Features.Auth.Commands.Login;

namespace CodeCafe.ApiService.Features.Auth.Handlers.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtTokenService _tokenService;
    private readonly IMediator _mediator;

    public LoginCommandHandler(
        IUsuarioRepository usuarioRepository,
        IJwtTokenService tokenService,
        IMediator mediator)
    {
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
        _mediator = mediator;
    }

    public async Task<AuthResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorEmailAsync(command.Email);

        if (usuario == null)
        {
            throw new SecurityException("E-mail não cadastrado.");
        }

        if (!usuario.VerificarSenha(command.Senha))
        {
            throw new SecurityException("Algo não está correto, precisa de ajuda?");
        }
        
        // Verificar se o email foi confirmado
        if (!usuario.EmailConfirmado)
        {
            throw new SecurityException("É necessário confirmar seu email antes de fazer login. Verifique sua caixa de entrada.");
        }
        
        // Verificar se 2FA está habilitado
        if (usuario.TwoFactorHabilitado)
        {
            // Se 2FA está habilitado, não permitimos login diretamente
            // Em vez disso, retornamos uma resposta especial indicando que é necessário 2FA
            return new AuthResponse
            {
                RequiresTwoFactor = true,
                Email = usuario.Email
                // Não incluir token ou dados do usuário aqui
            };
        }

        // Prosseguir com o login normal (sem 2FA)
        var token = _tokenService.GenerateToken(usuario);

        await _mediator.Publish(new UsuarioLogadoEvent(
            usuario.UsuarioId,
            DateTime.UtcNow
        ), cancellationToken);

        return new AuthResponse(
            token, 
            new UsuarioDto(usuario.UsuarioId, usuario.Nome, usuario.Email, usuario.DataNascimento)
        );
    }
}