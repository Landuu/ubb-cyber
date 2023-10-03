using System.Diagnostics;
using ubb_cyber.Models;
using ubb_cyber.Services.UserService;

namespace ubb_cyber.Services.ValidatorUserProvider
{
    public class ValidatorUserProvider : IValidatorUserProvider
    {
        private readonly IUserService _userService;
        public User? User { get; private set; }
        public UserPasswordPolicy? PasswordPolicy { get; private set; }

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
            Debug.WriteLine(User == null ? "TRUE" : "FALSE");
            Debug.WriteLine(PasswordPolicy == null ? "PP TRUE" : "PP FALSE");
            User ??= await _userService.GetUserFromRequest(cancellationToken);
            return User;
        }

        public async Task<User?> GetUserByResetKey(string? resetKey, CancellationToken cancellationToken)
        {
            if(resetKey == null) return null;
            User ??= await _userService.GetUserByKey(resetKey, cancellationToken);
            return User;
        }

        public async Task<UserPasswordPolicy?> GetUserPasswordPolicy(CancellationToken cancellationToken)
        {
            if(User == null) return null;
            PasswordPolicy ??= await _userService.GetUserPasswordPolicy(User.Id, cancellationToken);
            return PasswordPolicy;
        }
    }
}
