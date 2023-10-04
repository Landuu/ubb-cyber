using ubb_cyber.Services.UserService;

namespace ubb_cyber.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string? CurrentPassword { get; set; }

        public string? Password { get; set; }

        public string? PasswordConfirm { get; set; }

        public UserPasswordPolicy? PasswordPolicy { get; set; }
    }
}
