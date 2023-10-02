using FluentValidation;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Validators
{
    public class PanelPolicyViewModelValidator : AbstractValidator<PanelPolicyViewModel>
    {
        public PanelPolicyViewModelValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.MinPasswordLength)
                .NotEmpty()
                    .WithName("Minimalna długość hasła")
                .GreaterThanOrEqualTo(2)
                    .WithName("Minimalna długość hasła")
                .LessThanOrEqualTo(32)
                    .WithName("Minimalna długość hasła");

            RuleFor(x => x.PasswordExpireDays)
                .NotEmpty()
                    .WithName("Ważność hasła (dni)")
                .GreaterThanOrEqualTo(1)
                    .WithName("Ważność hasła (dni)")
                .LessThanOrEqualTo(366)
                    .WithName("Ważność hasła (dni)");

            RuleFor(x => x.UppercaseCount)
                .NotEmpty()
                    .WithName("Minmalna ilość dużych liter")
                .GreaterThanOrEqualTo(0)
                    .WithName("Minmalna ilość dużych liter")
                .LessThanOrEqualTo(8)
                    .WithName("Minmalna ilość dużych liter");

            RuleFor(x => x.NumbersCount)
                .NotEmpty()
                    .WithName("Minimalna ilość cyfr")
                .GreaterThanOrEqualTo(0)
                    .WithName("Minimalna ilość cyfr")
                .LessThanOrEqualTo(8)
                    .WithName("Minimalna ilość cyfr");
        }
    }
}
