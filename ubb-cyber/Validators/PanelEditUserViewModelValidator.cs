using FluentValidation;
using ubb_cyber.Database;
using ubb_cyber.Services.UserService;
using ubb_cyber.Services.ValidatorUserProvider;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Validators
{
    public class PanelEditUserViewModelValidator : AbstractValidator<PanelEditUserViewModel>
    {
        private readonly IUserService _userService;
        private readonly IValidatorUserProvider _userProvider;

        public PanelEditUserViewModelValidator(IUserService userService, IValidatorUserProvider userProvider)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            _userService = userService;
            _userProvider = userProvider;

            RuleFor(model => model.Login)
                .NotEmpty()
                .MinimumLength(4)
                .MustAsync(async (model, login, cancellationToken) =>
                {
                    var user = await _userProvider.GetUserById(model.Id, cancellationToken);
                    if (user == null || login == null) return false;
                    if (user.Login == login) return true;
                    return !await _userService.IsUserByLogin(login, cancellationToken);
                }).WithMessage("Istnieje już użytkownik o podanym loginie");

            RuleFor(model => model.NewPassword)
                .MinimumLength(6)
                    .When(model => model.NewPassword != null)
                    .WithName("Hasło");

            RuleFor(model => model.NewPasswordConfirm)
                .Must((model, confirm) => model.NewPassword == confirm)
                    .When(model => model.NewPassword != null)
                    .WithMessage("Hasła muszą być takie same");

            RuleFor(model => model.Locked)
                .NotNull();

            // Policies
            RuleFor(x => x.OverrideMinPasswordLength)
                .GreaterThanOrEqualTo(2)
                    .When(x => x.OverrideMinPasswordLength != null)
                    .WithName("Minimalna długość hasła")
                .LessThanOrEqualTo(32)
                    .When(x => x.OverrideMinPasswordLength != null)
                    .WithName("Minimalna długość hasła");

            RuleFor(x => x.OverridePasswordExpireDays)
                .GreaterThanOrEqualTo(1)
                    .WithName("Ważność hasła (dni)")
                    .When(x => x.OverrideMinPasswordLength != null)
                .LessThanOrEqualTo(9999)
                    .WithName("Ważność hasła (dni)")
                    .When(x => x.OverrideMinPasswordLength != null);

            RuleFor(x => x.OverrideUppercaseCount)
                .GreaterThanOrEqualTo(0)
                    .WithName("Minmalna ilość dużych liter")
                    .When(x => x.OverrideMinPasswordLength != null)
                .LessThanOrEqualTo(8)
                    .WithName("Minmalna ilość dużych liter")
                    .When(x => x.OverrideMinPasswordLength != null);

            RuleFor(x => x.OverrideNumbersCount)
                .GreaterThanOrEqualTo(0)
                    .WithName("Minimalna ilość cyfr")
                    .When(x => x.OverrideMinPasswordLength != null)
                .LessThanOrEqualTo(8)
                    .WithName("Minimalna ilość cyfr")
                    .When(x => x.OverrideMinPasswordLength != null);
        }
    }
}
