using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ubb_cyber.Database;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Validators
{
    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        private const string _errorMessage = "Nieprawidłowy login lub hasło";

        public LoginViewModelValidator(AppDbContext context)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(model => model.Login)
                .NotNull()
                .MinimumLength(1)
                    .WithMessage(_errorMessage)
                .MustAsync(async (model, login, cancellationToken) =>
                {
                    return await ValidateUser(context, login, model.Password, cancellationToken);
                })
                    .WithMessage(_errorMessage);

            RuleFor(model => model.Password)
                .NotNull()
                    .WithName("Hasło")
                .MinimumLength(1)
                    .WithMessage(_errorMessage)
                .MustAsync(async (model, password, cancellationToken) =>
                {
                    return await ValidateUser(context, model.Login, password, cancellationToken);
                })
                    .WithMessage(_errorMessage);
        }

        private static async Task<bool> ValidateUser(AppDbContext context, string? login, string? password, CancellationToken cancellationToken)
        {
            if (login == null || password == null) return false;
            var user = await context.Users.FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
            if (user == null) return false;
            return BCrypt.Net.BCrypt.EnhancedVerify(password, user.PasswordHash);
        }
    }
}
