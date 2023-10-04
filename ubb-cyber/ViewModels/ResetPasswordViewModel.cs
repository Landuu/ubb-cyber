using ubb_cyber.Services.UserService;

namespace ubb_cyber.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string? Key { get; set; }

        public string? CurrentPassword { get; set; }

        public string? Password { get; set; }

        public string? PasswordConfirm { get; set; }

        public UserPasswordPolicy? PasswordPolicy { get; set; }
    }
}
