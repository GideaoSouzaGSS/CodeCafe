using System.Security.Cryptography;
using System.Text;
using CodeCafe.Application.Interfaces.Services;
using Google.Authenticator;
using Microsoft.Extensions.Logging;

namespace CodeCafe.Infrastructure.Services;

public class TwoFactorService : ITwoFactorService
{
    private const string Issuer = "CodeCafe";
    private readonly ILogger<TwoFactorService> _logger;

    public TwoFactorService(ILogger<TwoFactorService> logger)
    {
        _logger = logger;
    }

    public string GerarChaveSecreta()
    {
        try
        {
            // Gerar bytes aleatórios para usar como chave secreta
            var randomBytes = new byte[20];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            
            // Converter para string Base32 compatível com autenticadores
            return Base32Encode(randomBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar chave secreta para 2FA");
            throw;
        }
    }
    
    public bool ValidarCodigo(string chaveSecreta, string codigoInformado)
    {
        if (string.IsNullOrEmpty(chaveSecreta) || string.IsNullOrEmpty(codigoInformado))
            return false;
            
        try
        {
            var tfa = new TwoFactorAuthenticator();
            var result = tfa.ValidateTwoFactorPIN(
                chaveSecreta, 
                codigoInformado, 
                TimeSpan.FromSeconds(30));
                
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar código 2FA");
            return false;
        }
    }
    
    public string GerarQrCodeUrl(string email, string chaveSecreta)
    {
        try
        {
            var tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode(
                Issuer, 
                email, 
                chaveSecreta, 
                false, 
                3);
                
            // Retorna a URL da imagem QR code
            return setupInfo.QrCodeSetupImageUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar QR code para 2FA");
            throw;
        }
    }
    
    // Helper method to Base32 encode bytes (for compatibility with Google Authenticator)
    private static string Base32Encode(byte[] data)
    {
        // RFC 4648 Base32 alphabet
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        
        var bits = 0;
        var value = 0;
        var output = new StringBuilder();
        
        for (var i = 0; i < data.Length; i++)
        {
            value = (value << 8) | data[i];
            bits += 8;
            
            while (bits >= 5)
            {
                output.Append(alphabet[(value >> (bits - 5)) & 31]);
                bits -= 5;
            }
        }
        
        if (bits > 0)
        {
            output.Append(alphabet[(value << (5 - bits)) & 31]);
        }
        
        return output.ToString();
    }
}