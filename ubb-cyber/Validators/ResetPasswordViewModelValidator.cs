using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ubb_cyber.Database;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Validators
{
    public class ResetPasswordViewModelValidator : AbstractValidator<ResetPasswordViewModel>
    {
        public ResetPasswordViewModelValidator(AppDbContext context)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(model => model.Key)
                .NotEmpty();

            RuleFor(model => model.CurrentPassword)
                .NotEmpty()
                    .WithName("Obecne hasło")
                .MustAsync(async (model, password, cancellationToken) =>
                {
                    return await ValidateUser(context, model.Key, password, cancellationToken);
                })
                    .WithMessage("Obecne hasło jest niepoprawne");

            RuleFor(model => model.Password)
                .NotEmpty()
                    .WithName("Nowe hasło")
                .MinimumLength(6)
                    .WithName("Nowe hasło")
                .MaximumLength(32)
                    .WithName("Nowe hasło");

            RuleFor(model => model.PasswordConfirm)
                .NotEmpty()
                    .WithName("Potwierdź nowe hasło")
                .Must((model, confirm) => model.Password == confirm)
                    .WithMessage("Hasła muszą być takie same");

        }

        private static async Task<bool> ValidateUser(AppDbContext context, string? key, string? password, CancellationToken cancellationToken)
        {
            if (key == null || password == null) return false;
            var user = await context.Users.FirstOrDefaultAsync(x => x.ResetPasswordKey == key, cancellationToken);
            if (user == null) return false;
            return BCrypt.Net.BCrypt.EnhancedVerify(password, user.PasswordHash);
        }
    }
}
