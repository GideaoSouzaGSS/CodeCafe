using MediatR;
using CodeCafe.ApiService.Features.Usuarios.Models;

namespace CodeCafe.ApiService.Features.Usuarios.Queries;

public record GetUsuarioByIdQuery(Guid UsuarioId) : IRequest<UsuarioDto>;