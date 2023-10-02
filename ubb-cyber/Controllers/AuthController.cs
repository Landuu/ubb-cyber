using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordGenerator;
using System.Security.Claims;
using ubb_cyber.Database;
using ubb_cyber.Models;
using ubb_cyber.Services.UserService;
using ubb_cyber.Utils;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly IValidator<LoginViewModel> _loginValidator;
        private readonly IValidator<ResetPasswordViewModel> _resetValidator;
        private readonly IValidator<ChangePasswordViewModel> _changeValidator;

        public AuthController(AppDbContext context, IUserService userService, IValidator<LoginViewModel> loginValidator, IValidator<ResetPasswordViewModel> resetValidator, IValidator<ChangePasswordViewModel> changeValidator)
        {
            _context = context;
            _userService = userService;
            _loginValidator = loginValidator;
            _resetValidator = resetValidator;
            _changeValidator = changeValidator;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return RedirectToIndex();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (_userService.IsLoggedIn())
                return RedirectToIndex();

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel viewModel)
        {
            var result = await _loginValidator.ValidateAsync(viewModel);

            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return View(viewModel);
            }

            var user = await _userService.GetUserByLoginSingle(viewModel.Login!);

            // Check if password is expired
            if (user.PasswordExpireDate >= DateTime.Now)
            {
                user.ResetPasswordKey = _userService.GenerateResetPasswordKey();
                await _context.SaveChangesAsync();
            }

            // Redirect if reset password is present
            if(user.ResetPasswordKey != null)
            {
                return RedirectToAction("ResetPassword", new { key = user.ResetPasswordKey });
            }

            string role = user.Login.ToLower() == "admin" ? "admin" : "user";
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, role)
            };

            user.LastLogin = DateTime.Now;
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            await _context.SaveChangesAsync();
            return RedirectToIndex();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromQuery] string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return RedirectToIndex();

            var user = await _userService.GetUserByKey(key);
            if (user == null)
                return RedirectToIndex();

            var viewModel = new ResetPasswordViewModel()
            {
                Key = key
            };
            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel viewModel)
        {
            var result = await _resetValidator.ValidateAsync(viewModel);

            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return View(viewModel);
            }

            var user = await _userService.GetUserByKeySingle(viewModel.Key!);
            await _context.UsedPasswords.AddAsync(new UsedPassword()
            {
                UserId = user.Id,
                PasswordHash = user.PasswordHash,
                ChangeDate = DateTime.Now
            });
            user.PasswordHash = _userService.GeneratePasswordHash(viewModel.Password!);
            user.ResetPasswordKey = null;
            await _context.SaveChangesAsync();
            return RedirectToIndex();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToIndex();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordViewModel viewModel)
        {
            var result = await _changeValidator.ValidateAsync(viewModel);

            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return View(viewModel);
            }

            var user = await _userService.GetUserFromRequest();
            if (user == null) return RedirectToIndex();

            await _context.UsedPasswords.AddAsync(new UsedPassword()
            {
                UserId = user.Id,
                PasswordHash = user.PasswordHash,
                ChangeDate = DateTime.Now
            });
            user.PasswordHash = _userService.GeneratePasswordHash(viewModel.Password!);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Profile));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Seed()
        {
            var users = new List<User>
            {
                new User()
                {
                    Login = "admin",
                    PasswordHash = _userService.GeneratePasswordHash("admin")
                },

                new User()
                {
                    Login = "user",
                    PasswordHash = _userService.GeneratePasswordHash("user"),
                    ResetPasswordKey = _userService.GenerateResetPasswordKey()
                }
            };

            var passwordPolicy = new PasswordPolicy()
            {
                Key = SecurityPolicies.STANDARD,
                MinPasswordLength = 6,
                NumbersCount = 2,
                UppercaseCount = 1,
                PasswordExpireDays = 10
            };

            await _context.PasswordPolicies.ExecuteDeleteAsync();
            await _context.Users.ExecuteDeleteAsync();
            await _context.PasswordPolicies.AddAsync(passwordPolicy);
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
            return Ok("Updated database");
        }

        private IActionResult RedirectToIndex()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
