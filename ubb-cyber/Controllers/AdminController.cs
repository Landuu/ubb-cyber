using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Permissions;
using ubb_cyber.Database;
using ubb_cyber.Models;
using ubb_cyber.Services.UserService;
using ubb_cyber.Utils;
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
        private readonly IValidator<PanelPolicyViewModel> _policyValidator;

        public AdminController(
            AppDbContext context, 
            IMapper mapper, 
            IUserService userService, 
            IValidator<PanelAddUserViewModel> addUserValidator, 
            IValidator<PanelEditUserViewModel> editUserValidator,
            IValidator<PanelPolicyViewModel> policyValidator
            )
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _addUserValidator = addUserValidator;
            _editUserValidator = editUserValidator;
            _policyValidator = policyValidator;
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
            user.OverrideMinPasswordLength = viewModel.OverrideMinPasswordLength;
            user.OverridePasswordExpireDays = viewModel.OverridePasswordExpireDays;
            user.OverrideUppercaseCount = viewModel.OverrideUppercaseCount;
            user.OverrideNumbersCount = viewModel.OverrideNumbersCount;

            await _context.LoginEvents.AddAsync(new LoginEvent()
            {
                InsertDate = DateTime.Now,
                Action = LoginEventAction.EDIT_USER,
                UserLogin = user.Login
            });

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

            await _context.LoginEvents.AddAsync(new LoginEvent()
            {
                InsertDate = DateTime.Now,
                Action = LoginEventAction.ADD_USER,
                UserLogin = user.Login
            });

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToUsers();
        }

        [HttpPost("[controller]/Users/Delete")]
        public async Task<IActionResult> DeleteUser([FromQuery] int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return BadRequest();

            await _context.Users.Where(x => x.Id == userId).ExecuteDeleteAsync();
            
            await _context.LoginEvents.AddAsync(new LoginEvent()
            {
                InsertDate = DateTime.Now,
                Action = LoginEventAction.DELETE_USER,
                UserLogin = user.Login
            });

            return RedirectToUsers();
        }

        public async Task<IActionResult> Policy()
        {
            var policy = await _userService.GetPasswordPolicy();
            if (policy == null) return RedirectToIndex();
            var viewModel = _mapper.Map<PanelPolicyViewModel>(policy);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Policy([FromForm] PanelPolicyViewModel viewModel)
        {
            var result = await _policyValidator.ValidateAsync(viewModel);

            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return View(viewModel);
            }

            var policy = await _userService.GetPasswordPolicy();
            _mapper.Map(viewModel, policy);
            await _context.SaveChangesAsync();
            return RedirectToIndex();
        }

        public async Task<IActionResult> Events()
        {
            var events = await _context.LoginEvents
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            return View(events);
        }

        private IActionResult RedirectToUsers()
        {
            return RedirectToAction("Users");
        }

        private IActionResult RedirectToIndex()
        {
            return RedirectToAction("Index");
        }
    }
}
