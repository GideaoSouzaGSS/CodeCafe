using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using CodeCafe.ApiService.Features.Usuarios.Models;
using CodeCafe.Data;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using CodeCafe.Domain.Interfaces;
using CodeCafe.ApiService.Features.Auth.Commands.Login;
using CodeCafe.ApiService.Features.Auth.Handlers.Register;
using CodeCafe.Application.Interfaces.Services;

namespace CodeCafe.Tests.Unit.Features.Auth.Handlers;

public class RegisterCommandHandlerTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IValidator<RegisterCommand>> _validatorMock;
    private readonly RegisterCommandHandler _sut;
    private readonly Mock<IEmailService> _emailServiceMock;

    public RegisterCommandHandlerTests()
    {
        // Configurar o banco em memória
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new AppDbContext(options);
        _usuarioRepository = new UsuarioRepository(_dbContext);
        _mediatorMock = new Mock<IMediator>();
        _validatorMock = new Mock<IValidator<RegisterCommand>>();
        _emailServiceMock = new Mock<IEmailService>();

        // Setup padrão do validator para passar na validação
        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(new List<ValidationFailure>())); // Lista vazia significa sucesso na validação

        // Criar o handler com o repository, validator e email service
        // _sut = new RegisterCommandHandler( 
        //     _usuarioRepository, 
        //     _mediatorMock.Object,
        //     _validatorMock.Object,
        //     _emailServiceMock.Object
        // );
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldRegisterUserAndReturnId()
    {
        // Arrange
        var command = new RegisterCommand(
            "Test User",
            "test@example.com",
            "Password123!",
            DateOnly.FromDateTime(DateTime.Now.AddYears(-20))
        );

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);

        // Verificar se o usuário foi salvo usando o repository
        var usuarios = await _usuarioRepository.GetUsuariosAsync();
        var savedUser = usuarios.Should().ContainSingle().Subject;
        
        savedUser.UsuarioId.Should().Be(result);
        savedUser.Nome.Should().Be(command.Nome);
        savedUser.Email.Should().Be(command.Email);
        savedUser.DataNascimento.Should().Be(command.DataNascimento);

        // Verificar publicação do evento
        _mediatorMock.Verify(
            x => x.Publish(
                It.Is<UsuarioRegistradoEvent>(e =>
                    e.UsuarioId == result &&
                    e.Email == command.Email),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ShouldThrowException()
    {
        // Arrange
        var existingEmail = "existing@example.com";
        var command1 = new RegisterCommand(
            "First User",
            existingEmail,
            "Password123!",
            DateOnly.FromDateTime(DateTime.Now.AddYears(-20))
        );

        var command2 = new RegisterCommand(
            "Second User",
            existingEmail,
            "Password456!",
            DateOnly.FromDateTime(DateTime.Now.AddYears(-25))
        );

        // Registrar primeiro usuário
        await _sut.Handle(command1, CancellationToken.None);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.Handle(command2, CancellationToken.None));
    }

    [Theory]
    [InlineData("", "test@example.com", "Password123!", "2000-01-01")]
    [InlineData("Test User", "", "Password123!", "2000-01-01")]
    [InlineData("Test User", "test@example.com", "", "2000-01-01")]
    [InlineData("Test User", "invalid-email", "Password123!", "2000-01-01")]
    public async Task Handle_WithInvalidData_ShouldThrowValidationException(
        string nome, string email, string senha, string dataNascimento)
    {
        // Arrange
        var command = new RegisterCommand(
            nome,
            email,
            senha,
            DateOnly.Parse(dataNascimento));

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Property", "Validation error message")
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationFailures));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(
            () => _sut.Handle(command, CancellationToken.None));

        exception.Errors.Should().NotBeEmpty();
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}