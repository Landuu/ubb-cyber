using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ubb_cyber.Database;
using ubb_cyber.ViewModels;

namespace ubb_cyber.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AdminController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        [Route("[controller]/Users/Add")]
        public IActionResult AddUser()
        {
            return View();
        }

        public IActionResult RedirectToUsers()
        {
            return RedirectToAction("Users");
        }
    }
}
