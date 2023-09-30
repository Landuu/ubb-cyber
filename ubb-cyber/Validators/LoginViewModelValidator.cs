using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ubb_cyber.Database;
using ubb_cyber.Services.UserService;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Validators
{
    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        private const string _errorMessage = "Nieprawidłowy login lub hasło";
        private readonly IUserService _userService;
        private readonly AppDbContext _context;

        public LoginViewModelValidator(AppDbContext context, IUserService userService)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            _context = context;
            _userService = userService;

            RuleFor(model => model.Login)
                .NotNull()
                .MinimumLength(1)
                    .WithMessage(_errorMessage)
                .MustAsync(async (model, login, cancellationToken) =>
                {
                    return await ValidateUser(login, model.Password, cancellationToken);
                })
                    .WithMessage(_errorMessage)
                .MustAsync(async (model, login, cancellationToken) =>
                 {
                     return !await _userService.IsUserLocked(login, cancellationToken);
                 })
                    .WithMessage("Podane konto jest zablokowane");


            RuleFor(model => model.Password)
                .NotNull()
                    .WithName("Hasło")
                .MinimumLength(1)
                    .WithMessage(_errorMessage)
                .MustAsync(async (model, password, cancellationToken) =>
                {
                    return await ValidateUser(model.Login, password, cancellationToken);
                })
                    .WithMessage(_errorMessage);
        }

        private async Task<bool> ValidateUser(string? login, string? password, CancellationToken cancellationToken)
        {
            if (login == null || password == null) return false;
            var user = await _userService.GetUserByLogin(login, cancellationToken);
            if (user == null) return false;
            return _userService.ValidatePasswordHash(password, user.PasswordHash);
        }
    }
}
