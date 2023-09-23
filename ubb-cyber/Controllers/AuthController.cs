using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ubb_cyber.ViewModels;
using ubb_cyber.Models;
using System.Runtime.CompilerServices;
using ubb_cyber.Database;
using Microsoft.EntityFrameworkCore;

namespace ubb_cyber.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToIndex();
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToIndex();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel viewModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == viewModel.Login);
            if(user == null)
            {
                return View();
            }

            bool passwordValid = ValidatePassword(user.PasswordHash, viewModel.Password);
            if (!passwordValid)
            {
                return View();
            }

            string role = user.Login.ToLower() == "admin" ? "admin" : "user";
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToIndex();
        }

        [HttpGet]
        public IActionResult Check()
        {
            var name = User.Identity?.Name ?? "Not logged-in";
            return Ok(name);
        }


        [HttpGet]
        public async Task<IActionResult> Seed()
        {
            var users = new List<User>
            {
                new User()
                {
                    Login = "admin",
                    PasswordHash = GetPasswordHash("admin")
                },

                new User()
                {
                    Login = "user",
                    PasswordHash = GetPasswordHash("user")
                }
            };

            await _context.Users.ExecuteDeleteAsync();
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
            return Ok("Updated users");
        }

        private IActionResult RedirectToIndex()
        {
            return RedirectToAction("Index", "Home");
        }

        private static string GetPasswordHash(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        private static bool ValidatePassword(string hash, string password)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
        }
    }
}
