using FluentValidation;
using System.Threading;
using ubb_cyber.Database;
using ubb_cyber.Migrations;
using ubb_cyber.Models;
using ubb_cyber.Services.UserService;
using ubb_cyber.Services.ValidatorUserProvider;
using ubb_cyber.Utils;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Validators
{
    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        private const string _invalidCredentialsError = "Nieprawidłowy login lub hasło";
        private const int _invalidPasswordMaxAttempts = 5;
        private const int _invalidPasswordLockoutMinutes = 15;
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly IValidatorUserProvider _userProvider;

        public LoginViewModelValidator(IUserService userService, IValidatorUserProvider userProvider, AppDbContext context)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            _context = context;
            _userService = userService;
            _userProvider = userProvider;

            RuleFor(model => new { model.Login, model.Password, model.Otp, model.OtpX, model.CaptchaSubtract, model.CaptchaA,  model.CaptchaB, model.CaptchaAnswer })
                .Configure(c => c.PropertyName = "Login")
                .Must(model =>
                {
                    if (model.CaptchaA == 0 || model.CaptchaB == 0 || model.CaptchaAnswer == null)
                        return false;

                    var result = model.CaptchaSubtract 
                        ? model.CaptchaA - model.CaptchaB 
                        : model.CaptchaA + model.CaptchaB;
                    return result == model.CaptchaAnswer;
                })
                    .WithMessage("Nieprawidłowa CAPTCHA")
                .MustAsync(async (model, cancellationToken) =>
                {
                    if (model.Login == null || model.Login.Length < 1)
                        return false;

                    var user = await _userProvider.GetUserByLogin(model.Login, cancellationToken);
                    if (user == null)
                        return false;

                    bool passwordValid = false;
                    if(user.Otp)
                    {
                        passwordValid = true;
                    }
                    else if(model.Password != null)
                    {
                        passwordValid = _userService.ValidatePasswordHash(model.Password, user.PasswordHash);
                    }
                    else
                    {
                        return false;
                    }

                    if (user.Locked && user.LockedUntilDate != null && user.InvalidPasswordCounter > 0)
                    {
                        // If user is locked, skip and go to locked account validation
                        if (user.LockedUntilDate >= DateTime.Now) return true;

                        // If lock is expired, unlock account
                        user.Locked = false;
                        user.InvalidPasswordCounter = 0;
                        user.LockedUntilDate = null;
                        await _context.SaveChangesAsync(cancellationToken);
                    }

                    if (passwordValid)
                    {
                        user.InvalidPasswordCounter = 0;
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    else
                    {
                        user.InvalidPasswordCounter++;
                        await _context.LoginEvents.AddAsync(new LoginEvent()
                        {
                            InsertDate = DateTime.Now,
                            Action = LoginEventAction.LOGIN_INVALID_PASSWORD,
                            UserLogin = user.Login,
                            Description = $"Attempt: {user.InvalidPasswordCounter}"
                        }, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                    }

                    if (user.InvalidPasswordCounter >= _invalidPasswordMaxAttempts)
                    {
                        user.Locked = true;
                        user.LockedUntilDate = DateTime.Now + TimeSpan.FromMinutes(_invalidPasswordLockoutMinutes);
                        await _context.LoginEvents.AddAsync(new LoginEvent()
                        {
                            InsertDate = DateTime.Now,
                            Action = LoginEventAction.LOGIN_ATTEMPTS_LOCKED,
                            UserLogin = user.Login,
                            Description = user.LockedUntilDate.Value.ToString("dd.MM.yyyy HH:mm")
                        }, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                        return true;
                    }

                    return passwordValid;
                })
                    .WithMessage(_invalidCredentialsError)
                .MustAsync(async (model, cancellationToken) =>
                {
                    if (model.Login == null)
                        return false;

                    var user = await _userProvider.GetUserByLogin(model.Login, cancellationToken);
                    if (user == null)
                        return false;

                    if (!user.Otp)
                        return true;

                    if (model.OtpX == null)
                        return false;

                    var otpA = model.Login.Length;
                    var otpVal = otpA * Math.Log(Convert.ToDouble(model.OtpX));
                    var otpCorrectValue = (int) otpVal;
                    var modelOtpInt = Convert.ToInt32(model.Otp);
                    bool otpIsValid = otpCorrectValue == modelOtpInt;

                    if(otpIsValid)
                    {
                        user.Otp = false;
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    return otpIsValid;
                })
                    .WithMessage("Nieprawidłowe hasło jednorazowe")
                .MustAsync(async (model, cancellationToken) =>
                {
                    var user = await _userProvider.GetUserByLogin(model.Login, cancellationToken);
                    if (user == null) return false;
                    bool lockedInvalidPassword = user.Locked && user.InvalidPasswordCounter > 0 && user.LockedUntilDate != null;

                    if(lockedInvalidPassword)
                    {
                        await _context.LoginEvents.AddAsync(new LoginEvent()
                        {
                            InsertDate = DateTime.Now,
                            Action = LoginEventAction.LOGIN_ERROR_LOCKED,
                            UserLogin = user.Login
                        }, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                    }

                    return !lockedInvalidPassword;
                })
                    .WithMessage("Podane konto jest zablokowane z powodu zbyt wielu nieudanych prób logowania")
                .MustAsync(async (model, cancellationToken) =>
                {
                    var user = await _userProvider.GetUserByLogin(model.Login, cancellationToken);
                    if (user == null) return false;
                    return !user.Locked;
                })
                    .WithMessage("Podane konto jest zablokowane");

        }
    }
}
