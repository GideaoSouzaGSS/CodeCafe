using CodeCafe.Domain.Entities;

namespace CodeCafe.Domain.Interfaces;
public interface IUsuarioRepository
{
    Task<List<Usuario>> GetUsuariosAsync();
    Task<bool> ExisteUsuarioComEmailAsync(string email);
    void AdicionarUsuario(Usuario usuario);
    Task SalvarAlteracoesAsync();
    Task<Usuario?> ObterUsuarioPorEmailAsync(string email);
    Task<Usuario?> ObterUsuarioPorIdAsync(Guid id);
    Task<Usuario> ObterPorEmailAsync(string email);
    Task<Usuario> ObterPorIdAsync(Guid id);
    Task AtualizarAsync(Usuario usuario);
}