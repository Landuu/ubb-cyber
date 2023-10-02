using FluentValidation;
using ubb_cyber.Database;
using ubb_cyber.Models;
using ubb_cyber.Services.UserService;
using ubb_cyber.Services.ValidatorUserProvider;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Validators
{
    public class ChangePasswordViewModelValidator : AbstractValidator<ChangePasswordViewModel>
    {
        private readonly IUserService _userService;
        private readonly IValidatorUserProvider _userProvider;

        public ChangePasswordViewModelValidator(IUserService userService, IValidatorUserProvider userProvider)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            _userService = userService;
            _userProvider = userProvider;

            RuleFor(model => model.CurrentPassword)
                .NotEmpty()
                    .WithName("Obecne hasło")
                .MustAsync(async (model, password, cancellationToken) =>
                {
                    var user = await _userProvider.GetUserByRequest(cancellationToken);
                    if (user == null || password == null) return false;
                    return _userService.ValidatePasswordHash(password, user.PasswordHash);
                }).WithMessage("Obecne hasło jest niepoprawne");

            RuleFor(model => model.Password)
                .NotEmpty()
                    .WithName("Nowe hasło")
                .MinimumLength(6)
                    .WithName("Nowe hasło")
                .MaximumLength(32)
                    .WithName("Nowe hasło")
                .MustAsync(async (model, password, cancellationToken) =>
                {
                    var user = await _userProvider.GetUserByRequest(cancellationToken);
                    if (user == null || password == null) return false;
                    var usedPasswords = await _userService.GetUsedPasswords(user.Id, cancellationToken);
                    foreach(var used in usedPasswords)
                    {
                        if (_userService.ValidatePasswordHash(password, used)) 
                            return false;
                    }
                    return true;
                }).WithMessage("Podane hasło było już kiedyś użyte");

            RuleFor(model => model.PasswordConfirm)
                .NotEmpty()
                    .WithName("Potwierdź nowe hasło")
                .Must((model, confirm) => model.Password == confirm)
                    .WithMessage("Hasła muszą być takie same");
        }
    }
}
