using FluentValidation;
using Microsoft.EntityFrameworkCore;
using CodeCafe.Data;
using CodeCafe.ApiService.Features.Auth.Commands.Login;

namespace CodeCafe.ApiService.Features.Auth.Validators.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(AppDbContext context)
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MinimumLength(3).WithMessage("Nome deve ter pelo menos 3 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.")
            .MustAsync(async (email, _) => !await context.Usuarios.AnyAsync(u => u.Email == email))
            .WithMessage("E-mail já cadastrado.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(8).WithMessage("Senha deve ter pelo menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula.")
            .Matches("[a-z]").WithMessage("Senha deve conter pelo menos uma letra minúscula.")
            .Matches("[0-9]").WithMessage("Senha deve conter pelo menos um número.");

        RuleFor(x => x.DataNascimento)
            .Must(data => data <= DateOnly.FromDateTime(DateTime.Today.AddYears(-18)))
            .WithMessage("Usuário deve ter pelo menos 18 anos.");
    }
}