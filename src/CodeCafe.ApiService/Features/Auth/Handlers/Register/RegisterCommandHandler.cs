using FluentValidation;
using MediatR;
using CodeCafe.Domain.Entities;
using CodeCafe.ApiService.Features.Usuarios.Models;
using CodeCafe.Domain.Interfaces;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.ApiService.Features.Auth.Commands.Login;
using MassTransit;
using CodeCafe.Messaging.Messages;

namespace CodeCafe.ApiService.Features.Auth.Handlers.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMediator _mediator;
    private readonly IValidator<RegisterCommand> _validator;
    private readonly IPublishEndpoint _publishEndpoint;

    public RegisterCommandHandler(
        IUsuarioRepository usuarioRepository,
        IMediator mediator,
        IValidator<RegisterCommand> validator,
        IPublishEndpoint publishEndpoint)
    {
        _usuarioRepository = usuarioRepository;
        _mediator = mediator;
        _validator = validator;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Validar o comando
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Verificar se já existe um usuário com o mesmo email usando o repository
        if (await _usuarioRepository.ExisteUsuarioComEmailAsync(request.Email))
        {
            throw new InvalidOperationException($"Já existe um usuário cadastrado com o email {request.Email}");
        }
        

        var usuario = new Usuario(
            request.Nome,
            request.Email,
            request.Senha,
            request.DataNascimento
        );
        // Gerar o código de confirmação de email
        usuario.GerarCodigoConfirmacaoEmail();

        _usuarioRepository.AdicionarUsuario(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        // Publicar evento em vez de enviar e-mail diretamente
        await _publishEndpoint.Publish(new UserRegisteredEvent
        {
            UserId = usuario.UsuarioId,
            Email = usuario.Email, 
            Username = usuario.Email, // Ou outro campo apropriado
            ConfirmationToken = usuario.CodigoConfirmacaoEmail ?? Guid.NewGuid().ToString()
        }, cancellationToken);


        // Enviar email de confirmação
        // await _emailService.EnviarEmailConfirmacaoAsync(
        //     usuario.Email,
        //     usuario.Nome,
        //     usuario.CodigoConfirmacaoEmail);

        // Dispara evento de registro
        await _mediator.Publish(new UsuarioRegistradoEvent(
            usuario.UsuarioId,
            usuario.Nome,
            usuario.Email,
            usuario.DataNascimento.ToDateTime(TimeOnly.MinValue),
            DateTime.UtcNow
        ), cancellationToken);

        return usuario.UsuarioId;
    }
}