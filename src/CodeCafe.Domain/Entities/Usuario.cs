using Microsoft.AspNetCore.Identity;


namespace CodeCafe.Domain.Entities;

public class Usuario
{
    public Guid UsuarioId { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; } // Armazenaremos o hash da senha
    public DateOnly DataNascimento { get; set; }
    
    // Propriedades para confirmação de email
    public bool EmailConfirmado { get; set; }
    public string? CodigoConfirmacaoEmail { get; set; }
    public DateTime? DataGeracaoCodigoEmail { get; set; }
    
    // Propriedades para autenticação de dois fatores (2FA)
    public bool TwoFactorHabilitado { get; set; }
    public string? ChaveAutenticacaoTwoFactor { get; set; }
    public List<string> TokensRecuperacaoTwoFactor { get; set; } = new();
    
    protected Usuario() { }

    public List<UsuarioRole> Roles { get; set; } = new();
    public Usuario(string nome, string email, string senha, DateOnly dataNascimento)
    {
        Nome = nome;
        Email = email;
        Senha = HashSenha(senha); // Gerar hash no construtor
        DataNascimento = dataNascimento;
        EmailConfirmado = false; // Por padrão, email não está confirmado
        TwoFactorHabilitado = false; // Por padrão, 2FA não está habilitado
        GerarCodigoConfirmacaoEmail(); // Gera código de confirmação inicial
    }

    private static string HashSenha(string senha)
    {
        // Implementação do hash (ex: BCrypt)
        return BCrypt.Net.BCrypt.HashPassword(senha);
    }

    public bool VerificarSenha(string senhaTentativa)
    {
        return BCrypt.Net.BCrypt.Verify(senhaTentativa, Senha);
    }
    
    // Métodos para confirmação de email
    public void GerarCodigoConfirmacaoEmail()
    {
        CodigoConfirmacaoEmail = Guid.NewGuid().ToString("N");
        DataGeracaoCodigoEmail = DateTime.UtcNow;
    }
    
    public bool ConfirmarEmail(string codigo)
    {
        if (EmailConfirmado || string.IsNullOrEmpty(CodigoConfirmacaoEmail)) 
            return false;
            
        if (CodigoConfirmacaoEmail == codigo)
        {
            EmailConfirmado = true;
            CodigoConfirmacaoEmail = null;
            return true;
        }
        
        return false;
    }
    
    // Métodos para autenticação de dois fatores
    public void HabilitarTwoFactor(string chave)
    {
        if (!EmailConfirmado) 
            throw new InvalidOperationException("O email deve ser confirmado antes de habilitar 2FA");
            
        ChaveAutenticacaoTwoFactor = chave;
        TwoFactorHabilitado = true;
        GerarTokensRecuperacao();
    }
    
    public void DesabilitarTwoFactor()
    {
        TwoFactorHabilitado = false;
        ChaveAutenticacaoTwoFactor = null;
        TokensRecuperacaoTwoFactor.Clear();
    }
    
    private void GerarTokensRecuperacao()
    {
        TokensRecuperacaoTwoFactor.Clear();
        for (int i = 0; i < 5; i++)
        {
            TokensRecuperacaoTwoFactor.Add(Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper());
        }
    }
    
    public bool ValidarTokenRecuperacao(string token)
    {
        if (!TwoFactorHabilitado) return false;
        
        if (TokensRecuperacaoTwoFactor.Contains(token))
        {
            TokensRecuperacaoTwoFactor.Remove(token);
            return true;
        }
        
        return false;
    }
}