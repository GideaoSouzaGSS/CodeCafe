using Microsoft.EntityFrameworkCore;
using CodeCafe.Data;
using CodeCafe.Domain.Entities;
using CodeCafe.Domain.Interfaces;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Usuario>> GetUsuariosAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task<bool> ExisteUsuarioComEmailAsync(string email)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email == email);
    }

    public async Task<Usuario?> ObterUsuarioPorEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario?> ObterUsuarioPorIdAsync(Guid id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public void AdicionarUsuario(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Usuario> ObterPorEmailAsync(string email)
    {
        // Replace with your actual data access logic
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario> ObterPorIdAsync(Guid id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task AtualizarAsync(Usuario usuario)
    {
        // Marca a entidade como modificada para que o EF Core atualize todos os campos
        _context.Entry(usuario).State = EntityState.Modified;
        
        // Salva as mudanças no banco de dados
        await _context.SaveChangesAsync();
    }
}