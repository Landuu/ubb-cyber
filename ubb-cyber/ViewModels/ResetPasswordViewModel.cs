namespace ubb_cyber.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string? Key { get; set; }

        public string? CurrentPassword { get; set; }

        public string? Password { get; set; }

        public string? PasswordConfirm { get; set; }
    }
}
