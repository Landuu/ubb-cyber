namespace ubb_cyber.ViewModels
{
    public class PanelAddUserViewModel
    {
        public required string Login { get; set; }

        public required string Password { get; set; }

        public required string PasswordConfirm { get; set; }

        public bool Otp { get; set; }
    }
}
