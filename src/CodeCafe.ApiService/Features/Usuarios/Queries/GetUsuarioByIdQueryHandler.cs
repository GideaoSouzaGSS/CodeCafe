using MediatR;
using CodeCafe.Data.Repositories;
using CodeCafe.ApiService.Features.Usuarios.Models;
using CodeCafe.Domain.Interfaces;

namespace CodeCafe.ApiService.Features.Usuarios.Queries;

public class GetUsuarioByIdQueryHandler : IRequestHandler<GetUsuarioByIdQuery, UsuarioDto>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public GetUsuarioByIdQueryHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioDto> Handle(GetUsuarioByIdQuery query, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorIdAsync(query.UsuarioId);

        return usuario is null 
            ? null 
            : new UsuarioDto(
                usuario.UsuarioId,
                usuario.Nome,
                usuario.Email,
                usuario.DataNascimento
            );
    }
}