using System.ComponentModel;

namespace ubb_cyber.ViewModels
{
    public class LoginViewModel
    {
        public string? Login { get; set; }

        [DisplayName("Hasło")]
        public string? Password { get; set; }
    }
}
