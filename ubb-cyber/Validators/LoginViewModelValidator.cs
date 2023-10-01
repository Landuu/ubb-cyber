using FluentValidation;
using ubb_cyber.Models;
using ubb_cyber.Services.UserService;
using ubb_cyber.Services.ValidatorUserProvider;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Validators
{
    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        private const string _errorMessage = "Nieprawidłowy login lub hasło";
        private readonly IUserService _userService;
        private readonly IValidatorUserProvider _userProvider;

        public LoginViewModelValidator(IUserService userService, IValidatorUserProvider userProvider)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            _userService = userService;
            _userProvider = userProvider;

            RuleFor(model => model.Login)
                .NotNull()
                .MinimumLength(1)
                    .WithMessage(_errorMessage)
                .MustAsync(async (model, login, cancellationToken) =>
                {
                    var user = await _userProvider.GetUserByLogin(login, cancellationToken);
                    if (user == null || model.Password == null) return false;
                    return _userService.ValidatePasswordHash(model.Password, user.PasswordHash);
                }).WithMessage(_errorMessage)
                .MustAsync(async (model, login, cancellationToken) =>
                {
                    var user = await _userProvider.GetUserByLogin(login, cancellationToken);
                    if (user == null) return false;
                    return !user.Locked;
                }).WithMessage("Podane konto jest zablokowane");


            RuleFor(model => model.Password)
                .NotNull()
                    .WithName("Hasło")
                .MinimumLength(1)
                    .WithMessage(_errorMessage)
                .MustAsync(async (model, password, cancellationToken) =>
                {
                    var user = await _userProvider.GetUserByLogin(model.Login, cancellationToken);
                    if (user == null || model.Password == null) return false;
                    return _userService.ValidatePasswordHash(model.Password, user.PasswordHash);
                }).WithMessage(_errorMessage);
        }
    }
}
