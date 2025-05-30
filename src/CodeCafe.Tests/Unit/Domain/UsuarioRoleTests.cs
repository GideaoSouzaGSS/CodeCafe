using System;
using Xunit;
using CodeCafe.Domain.Entities;

public class UsuarioRoleTests
{
    [Fact]
    public void CriarUsuarioRole_DeveSetarPropriedades()
    {
        var role = new UsuarioRole
        {
            Id = Guid.NewGuid(),
            RoleName = "Admin"
        };

        Assert.Equal("Admin", role.RoleName);
        Assert.NotEqual(Guid.Empty, role.Id);
    }
}