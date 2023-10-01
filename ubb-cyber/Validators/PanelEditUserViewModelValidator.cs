using FluentValidation;
using ubb_cyber.Database;
using ubb_cyber.Services.UserService;
using ubb_cyber.Services.ValidatorUserProvider;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Validators
{
    public class PanelEditUserViewModelValidator : AbstractValidator<PanelEditUserViewModel>
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly IValidatorUserProvider _userProvider;

        public PanelEditUserViewModelValidator(AppDbContext context, IUserService userService, IValidatorUserProvider userProvider)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            _context = context;
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
        }
    }
}
