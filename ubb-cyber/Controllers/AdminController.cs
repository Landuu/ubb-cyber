using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ubb_cyber.Database;
using ubb_cyber.Models;
using ubb_cyber.Services.UserService;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IValidator<PanelAddUserViewModel> _addUserValidator;
        private readonly IValidator<PanelEditUserViewModel> _editUserValidator;

        public AdminController(AppDbContext context, IMapper mapper, IUserService userService, IValidator<PanelAddUserViewModel> addUserValidator, IValidator<PanelEditUserViewModel> editUserValidator)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _addUserValidator = addUserValidator;
            _editUserValidator = editUserValidator;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .Where(x => x.Login != "admin")
                .ToListAsync();
            var viewModel = _mapper.Map<List<PanelUserViewModel>>(users);
            return View(viewModel);
        }

        [Route("[controller]/Users/Edit")]
        public async Task<IActionResult> EditUser([FromQuery] int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return RedirectToUsers();
            var viewModel = _mapper.Map<PanelEditUserViewModel>(user);
            return View(viewModel);
        }

        [HttpPost("[controller]/Users/Edit")]
        public async Task<IActionResult> EditUser([FromForm] PanelEditUserViewModel viewModel)
        {
            var result = await _editUserValidator.ValidateAsync(viewModel);

            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return View(viewModel);
            }

            var user = await _userService.GetUserByIdSingle(viewModel.Id);
            if(user.Login != viewModel.Login) 
                user.Login = viewModel.Login;
            if(!string.IsNullOrWhiteSpace(viewModel.NewPassword)) 
                user.PasswordHash = _userService.GeneratePasswordHash(viewModel.NewPassword);
            user.Locked = viewModel.Locked;

            await _context.SaveChangesAsync();
            return RedirectToUsers();
        }

        [Route("[controller]/Users/Add")]
        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost("[controller]/Users/Add")]
        public async Task<IActionResult> AddUser([FromForm] PanelAddUserViewModel viewModel)
        {
            var result = await _addUserValidator.ValidateAsync(viewModel);

            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return View(viewModel);
            }

            var user = new User()
            {
                Login = viewModel.Login,
                PasswordHash = _userService.GeneratePasswordHash(viewModel.Password),
                ResetPasswordKey = _userService.GenerateResetPasswordKey()
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToUsers();
        }

        [HttpPost("[controller]/Users/Delete")]
        public async Task<IActionResult> DeleteUser([FromQuery] int userId)
        {
            await _context.Users.Where(x => x.Id == userId).ExecuteDeleteAsync();
            return RedirectToUsers();
        }

        public IActionResult RedirectToUsers()
        {
            return RedirectToAction("Users");
        }
    }
}
