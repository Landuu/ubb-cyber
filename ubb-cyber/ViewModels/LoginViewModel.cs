using System.ComponentModel;

namespace ubb_cyber.ViewModels
{
    public class LoginViewModel
    {
        public string? Login { get; set; }

        [DisplayName("Hasło")]
        public string? Password { get; set; }

        public int? OtpX { get; set; }

        public string? Otp { get; set; }
    }
}
