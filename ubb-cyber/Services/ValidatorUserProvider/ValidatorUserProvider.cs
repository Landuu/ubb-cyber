using ubb_cyber.Models;
using ubb_cyber.Services.UserService;

namespace ubb_cyber.Services.ValidatorUserProvider
{
    public class ValidatorUserProvider : IValidatorUserProvider
    {
        private readonly IUserService _userService;
        public User? User { get; private set; }

        public ValidatorUserProvider(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<User?> GetUserById(int? id, CancellationToken cancellationToken)
        {
            if(!id.HasValue || id == null) return null;
            User ??= await _userService.GetUserById(id.Value, cancellationToken);
            return User;
        }

        public async Task<User?> GetUserByLogin(string? login, CancellationToken cancellationToken)
        {
            if (login == null) return null;
            User ??= await _userService.GetUserByLogin(login, cancellationToken);
            return User;
        }

        public async Task<User?> GetUserByRequest(CancellationToken cancellationToken)
        {
            User ??= await _userService.GetUserFromRequest(cancellationToken);
            return User;
        }

        public async Task<User?> GetUserByResetKey(string? resetKey, CancellationToken cancellationToken)
        {
            if(resetKey == null) return null;
            User ??= await _userService.GetUserByKey(resetKey, cancellationToken);
            return User;
        }
    }
}
