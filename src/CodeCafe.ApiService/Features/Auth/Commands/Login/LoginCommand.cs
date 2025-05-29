using CodeCafe.ApiService.Features.Auth.DTOs;
using MediatR;

namespace CodeCafe.ApiService.Features.Auth.Commands.Login;
public record LoginCommand(string Email, string Senha) : IRequest<AuthResponse>;