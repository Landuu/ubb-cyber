using ubb_cyber.Models;

namespace ubb_cyber.Services.UserService
{
    public interface IUserService
    {
        string GeneratePasswordHash(string password);
        Task<User?> GetUserByKey(string key);
        Task<User?> GetUserByKey(string key, CancellationToken cancellationToken);
        Task<User> GetUserByKeySingle(string key);
        Task<User?> GetUserByLogin(string login);
        Task<User?> GetUserByLogin(string login, CancellationToken cancellationToken);
        Task<User> GetUserByLoginSingle(string login);
        Task<User?> GetUserFromRequest();
        Task<User?> GetUserFromRequest(CancellationToken cancellationToken);
        bool IsLoggedIn();
        bool ValidatePasswordHash(string password, string passwordHash);
    }
}