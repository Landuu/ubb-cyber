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
                .MustAsync(async (model, password, cancellationToken) =>
                {
                    await _userProvider.GetUserByRequest(cancellationToken);
                    var policy = await _userProvider.GetUserPasswordPolicy(cancellationToken);
                    if (password == null || policy == null) return false;
                    if (password.Length < policy.MinPasswordLength || password.Length > 32) return false;
                    return true;
                }).WithMessage(model => $"Hasło musi mieć od {model.PasswordPolicy?.MinPasswordLength ?? 0} do 32 znaków")
                .MustAsync(async (model, password, cancellationToken) =>
                {
                    await _userProvider.GetUserByRequest(cancellationToken);
                    var policy = await _userProvider.GetUserPasswordPolicy(cancellationToken);
                    if (password == null || policy == null) return false;
                    int countUppercase = password.Count(char.IsUpper);
                    return countUppercase >= policy.UppercaseCount;
                }).WithMessage(model => $"Hasło musi zawierać co najmniej {model.PasswordPolicy?.UppercaseCount ?? 0} duże litery")
                .MustAsync(async (model, password, cancellationToken) =>
                {
                    await _userProvider.GetUserByRequest(cancellationToken);
                    var policy = await _userProvider.GetUserPasswordPolicy(cancellationToken);
                    if (password == null || policy == null) return false;
                    int countNumbers = password.Count(char.IsNumber);
                    return countNumbers >= policy.UppercaseCount;
                }).WithMessage(model => $"Hasło musi zawierać co najmniej {model.PasswordPolicy?.NumbersCount ?? 0} cyfry")
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
