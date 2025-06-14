namespace CodeCafe.ApiService.Features.Auth.Projections;

public class UsuarioProjection
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public DateTime DataNascimento { get; set; }
    public DateTime DataCriacao { get; set; }
}