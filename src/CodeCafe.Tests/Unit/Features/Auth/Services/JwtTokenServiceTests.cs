using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using FluentAssertions;
using CodeCafe.ApiService.Features.Auth.Services;
using CodeCafe.Domain.Entities;
using Xunit;

namespace CodeCafe.Tests.Unit.Features.Auth.Services;

public class JwtTokenServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IConfigurationSection> _jwtSectionMock;
    private readonly JwtTokenService _sut;
    private readonly string _jwtSecret = "your-256-bit-secret-key-here-make-it-long-enough";

    public JwtTokenServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _jwtSectionMock = new Mock<IConfigurationSection>();
        
        // Setup JWT section mock
        _jwtSectionMock.Setup(x => x.Value).Returns(_jwtSecret);
        
        _configurationMock
            .Setup(x => x.GetSection("Jwt:Secret"))
            .Returns(_jwtSectionMock.Object);
            
        _configurationMock
            .Setup(x => x["Jwt:ExpiresInHours"])
            .Returns("1");

        _sut = new JwtTokenService(_configurationMock.Object);
    }

    [Fact]
    public void GenerateToken_WithValidUser_ShouldReturnValidToken()
    {
        // Arrange
        var usuario = new Usuario(
            nome: "Test User",
            email: "test@example.com",
            senha: "Password123!",
            dataNascimento: DateOnly.FromDateTime(DateTime.Now.AddYears(-20))
        );

        // Act
        var token = _sut.GenerateToken(usuario);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();
        
        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
        
        validatedToken.Should().NotBeNull();
        principal.Should().NotBeNull();
        
        // Validate specific claims
        principal.FindFirst(ClaimTypes.NameIdentifier)?.Value.Should().Be(usuario.UsuarioId.ToString());
        principal.FindFirst(ClaimTypes.Email)?.Value.Should().Be(usuario.Email);
        principal.FindFirst(ClaimTypes.Name)?.Value.Should().Be(usuario.Nome);
    }

    [Fact]
    public void GenerateToken_WithUserRoles_ShouldIncludeRoleClaims()
    {
        // Arrange
        var usuario = new Usuario(
            nome: "Test User",
            email: "test@example.com",
            senha: "Password123!",
            dataNascimento: DateOnly.FromDateTime(DateTime.Now.AddYears(-20))
        );
        
        usuario.Roles.Add(new UsuarioRole { RoleName = "Admin" });
        usuario.Roles.Add(new UsuarioRole { RoleName = "User" });

        // Act
        var token = _sut.GenerateToken(usuario);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();
        
        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
        
        var roleClaims = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        roleClaims.Should().Contain("Admin");
        roleClaims.Should().Contain("User");
        roleClaims.Should().HaveCount(2);
    }

    [Fact]
    public void GenerateToken_ShouldSetCorrectExpiration()
    {
        // Arrange
        var usuario = new Usuario(
            nome: "Test User",
            email: "test@example.com",
            senha: "Password123!",
            dataNascimento: DateOnly.FromDateTime(DateTime.Now.AddYears(-20))
        );

        // Act
        var token = _sut.GenerateToken(usuario);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var expectedExpiration = DateTime.UtcNow.AddHours(1);
        jwtToken.ValidTo.Should().BeCloseTo(expectedExpiration, TimeSpan.FromMinutes(1));
    }

    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    }
}