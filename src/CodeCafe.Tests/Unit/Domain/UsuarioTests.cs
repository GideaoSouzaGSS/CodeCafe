using System;
using Xunit;
using CodeCafe.Domain.Entities;

public class UsuarioTests
{
    [Fact]
    public void CriarUsuario_DeveGerarCodigoConfirmacaoEmail()
    {
        var usuario = new Usuario("Nome", "email@teste.com", "Senha123!", DateOnly.FromDateTime(DateTime.Today.AddYears(-20)));
        Assert.False(usuario.EmailConfirmado);
        Assert.False(string.IsNullOrEmpty(usuario.CodigoConfirmacaoEmail));
    }

    [Fact]
    public void ConfirmarEmail_ComCodigoCorreto_DeveConfirmar()
    {
        var usuario = new Usuario("Nome", "email@teste.com", "Senha123!", DateOnly.FromDateTime(DateTime.Today.AddYears(-20)));
        var codigo = usuario.CodigoConfirmacaoEmail!;
        var resultado = usuario.ConfirmarEmail(codigo);
        Assert.True(resultado);
        Assert.True(usuario.EmailConfirmado);
    }

    [Fact]
    public void HabilitarTwoFactor_SemEmailConfirmado_DeveLancarExcecao()
    {
        var usuario = new Usuario("Nome", "email@teste.com", "Senha123!", DateOnly.FromDateTime(DateTime.Today.AddYears(-20)));
        Assert.Throws<InvalidOperationException>(() => usuario.HabilitarTwoFactor("chave"));
    }

    [Fact]
    public void HabilitarTwoFactor_ComEmailConfirmado_DeveHabilitar()
    {
        var usuario = new Usuario("Nome", "email@teste.com", "Senha123!", DateOnly.FromDateTime(DateTime.Today.AddYears(-20)));
        usuario.ConfirmarEmail(usuario.CodigoConfirmacaoEmail!);
        usuario.HabilitarTwoFactor("chave");
        Assert.True(usuario.TwoFactorHabilitado);
        Assert.NotNull(usuario.ChaveAutenticacaoTwoFactor);
        Assert.NotEmpty(usuario.TokensRecuperacaoTwoFactor);
    }

    [Fact]
    public void VerificarSenha_DeveRetornarTrueParaSenhaCorreta()
    {
        var usuario = new Usuario("Nome", "email@teste.com", "Senha123!", DateOnly.FromDateTime(DateTime.Today.AddYears(-20)));
        Assert.True(usuario.VerificarSenha("Senha123!"));
    }
}