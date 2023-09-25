using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ubb_cyber.Database;
using ubb_cyber.Services.UserService;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Validators
{
    public class ChangePasswordViewModelValidator : AbstractValidator<ChangePasswordViewModel>
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;

        public ChangePasswordViewModelValidator(AppDbContext context, IUserService userService)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            _context = context;
            _userService = userService;

            RuleFor(model => model.CurrentPassword)
                .NotEmpty()
                    .WithName("Obecne hasło")
                .MustAsync(async (model, password, cancellationToken) =>
                {
                    return await ValidateUser(password, cancellationToken);
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

        private async Task<bool> ValidateUser(string? password, CancellationToken cancellationToken)
        {
            if (password == null) return false;
            var user = await _userService.GetUserFromRequest(cancellationToken);
            if (user == null) return false;
            return _userService.ValidatePasswordHash(password, user.PasswordHash);
        }
    }
}
