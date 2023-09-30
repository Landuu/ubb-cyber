using FluentValidation;
using ubb_cyber.Services.UserService;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Validators
{
    public class PanelAddUserViewModelValidator : AbstractValidator<PanelAddUserViewModel>
    {
        private readonly IUserService _userService;

        public PanelAddUserViewModelValidator(IUserService userService)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            _userService = userService;

            RuleFor(model => model.Login)
                .NotEmpty()
                .MinimumLength(4)
                .MustAsync(async (model, login, cancellationToken) => !await _userService.IsUserByLogin(login, cancellationToken))
                    .WithMessage("Istnieje już użytkownik o podanym loginie");

            RuleFor(model => model.Password)
                .NotEmpty()
                    .WithName("Hasło")
                .MinimumLength(6)
                    .WithName("Hasło");

            RuleFor(model => model.PasswordConfirm)
                .NotEmpty()
                    .WithName("Potwierdź hasło")
                .Must((model, confirm) => model.Password == confirm)
                    .WithMessage("Hasła muszą być takie same");
        }
    }
}
