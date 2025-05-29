namespace CodeCafe.Application.Interfaces.Services;

public interface ITwoFactorService
{
    /// <summary>
    /// Gera uma chave secreta para uso em autenticação de dois fatores
    /// </summary>
    /// <returns>Chave secreta codificada em Base32</returns>
    string GerarChaveSecreta();
    
    /// <summary>
    /// Valida um código TOTP com base na chave secreta
    /// </summary>
    /// <param name="chaveSecreta">Chave secreta do usuário</param>
    /// <param name="codigoInformado">Código informado pelo usuário</param>
    /// <returns>True se o código for válido, False caso contrário</returns>
    bool ValidarCodigo(string chaveSecreta, string codigoInformado);
    
    /// <summary>
    /// Gera uma URL para QR code que pode ser escaneada por aplicativos autenticadores
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <param name="chaveSecreta">Chave secreta do usuário</param>
    /// <returns>URL para imagem QR code</returns>
    string GerarQrCodeUrl(string email, string chaveSecreta);
}