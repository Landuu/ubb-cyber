using ubb_cyber.Models;

namespace ubb_cyber.Services.UserService
{
    public interface IUserService
    {
        string GeneratePasswordHash(string password);
        string GenerateResetPasswordKey();
        Task<PasswordPolicy?> GetPasswordPolicy();
        Task<List<string>> GetUsedPasswords(int userId, CancellationToken cancellationToken);
        Task<User?> GetUserById(int id);
        Task<User?> GetUserById(int id, CancellationToken cancellationToken);
        Task<User> GetUserByIdSingle(int id);
        Task<User?> GetUserByKey(string key);
        Task<User?> GetUserByKey(string key, CancellationToken cancellationToken);
        Task<User> GetUserByKeySingle(string key);
        Task<User?> GetUserByLogin(string login);
        Task<User?> GetUserByLogin(string login, CancellationToken cancellationToken);
        Task<User> GetUserByLoginSingle(string login);
        Task<User?> GetUserFromRequest();
        Task<User?> GetUserFromRequest(CancellationToken cancellationToken);
        bool IsLoggedIn();
        Task<bool> IsUserById(int id, CancellationToken cancellationToken);
        Task<bool> IsUserById(int id);
        Task<bool> IsUserByLogin(string login);
        Task<bool> IsUserByLogin(string login, CancellationToken cancellationToken);
        Task<bool> IsUserLocked(string login);
        Task<bool> IsUserLocked(string login, CancellationToken cancellationToken);
        bool ValidatePasswordHash(string password, string passwordHash);
    }
}