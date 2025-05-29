using MediatR;
using Moq;
using FluentAssertions;
using CodeCafe.ApiService.Features.Auth.Services;
using CodeCafe.Domain.Entities;
using System.Security;
using CodeCafe.ApiService.Features.Usuarios.Models;
using CodeCafe.Domain.Interfaces;
using CodeCafe.ApiService.Features.Auth.Commands.Login;
using CodeCafe.ApiService.Features.Auth.Handlers.Login;

namespace CodeCafe.Tests.Unit.Features.Auth.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IJwtTokenService> _jwtServiceMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _jwtServiceMock = new Mock<IJwtTokenService>();
        _mediatorMock = new Mock<IMediator>();

        _sut = new LoginCommandHandler(
            _usuarioRepositoryMock.Object,
            _jwtServiceMock.Object,
            _mediatorMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var nome = "Test User";
        var email = "test@example.com";
        var password = "Password123";

        var user = new Usuario(
            nome: nome,
            email: email,
            senha: password,
            dataNascimento: DateOnly.FromDateTime(DateTime.UtcNow)
        );
        
        // Configurar email como confirmado
        user.EmailConfirmado = true;

        _usuarioRepositoryMock
            .Setup(x => x.ObterUsuarioPorEmailAsync(email))
            .ReturnsAsync(user);

        var expectedToken = "test-jwt-token";
        _jwtServiceMock
            .Setup(x => x.GenerateToken(It.Is<Usuario>(u => u.Email == email)))
            .Returns(expectedToken);

        var command = new LoginCommand(email, password);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(expectedToken);
        result.Usuario.Email.Should().Be(email);
        result.Usuario.Nome.Should().Be(nome);

        // Verificar se o evento foi publicado
        _mediatorMock.Verify(
            x => x.Publish(
                It.Is<UsuarioLogadoEvent>(e => 
                    e.UsuarioId == user.UsuarioId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldThrowSecurityException()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "Password123";

        _usuarioRepositoryMock
            .Setup(x => x.ObterUsuarioPorEmailAsync(email))
            .ReturnsAsync((Usuario?)null);

        var command = new LoginCommand(email, password);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SecurityException>(
            () => _sut.Handle(command, CancellationToken.None));

        exception.Message.Should().Be("E-mail não cadastrado.");
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ShouldThrowSecurityException()
    {
        // Arrange
        var email = "test@example.com";
        var correctPassword = "Password123";
        var wrongPassword = "WrongPassword";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword);

        var user = new Usuario(
            nome: "Test User",
            email: email,
            senha: passwordHash,
            dataNascimento: DateOnly.FromDateTime(DateTime.UtcNow)
        );

        _usuarioRepositoryMock
            .Setup(x => x.ObterUsuarioPorEmailAsync(email))
            .ReturnsAsync(user);

        var command = new LoginCommand(email, wrongPassword);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SecurityException>(
            () => _sut.Handle(command, CancellationToken.None));

        exception.Message.Should().Be("Algo não está correto, precisa de ajuda?");
    }

    [Fact]
    public async Task Handle_WithUnconfirmedEmail_ShouldThrowSecurityException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123";

        var user = new Usuario(
            nome: "Test User",
            email: email,
            senha: password,
            dataNascimento: DateOnly.FromDateTime(DateTime.UtcNow)
        );
        
        // Email não confirmado
        user.EmailConfirmado = false;

        _usuarioRepositoryMock
            .Setup(x => x.ObterUsuarioPorEmailAsync(email))
            .ReturnsAsync(user);

        var command = new LoginCommand(email, password);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SecurityException>(
            () => _sut.Handle(command, CancellationToken.None));

        exception.Message.Should().Be("É necessário confirmar seu email antes de fazer login. Verifique sua caixa de entrada.");
    }
}