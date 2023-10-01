using ubb_cyber.Models;

namespace ubb_cyber.Services.ValidatorUserProvider
{
    public interface IValidatorUserProvider
    {
        User? User { get; }

        Task<User?> GetUserById(int? id, CancellationToken cancellationToken);
        Task<User?> GetUserByLogin(string? login, CancellationToken cancellationToken);
        Task<User?> GetUserByRequest(CancellationToken cancellationToken);
        Task<User?> GetUserByResetKey(string? resetKey, CancellationToken cancellationToken);
    }
}