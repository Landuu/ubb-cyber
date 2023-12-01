namespace ubb_cyber.ViewModels
{
    public class PanelEditUserViewModel
    {
        public int Id { get; set; }

        public required string Login { get; set; }

        public string? NewPassword { get; set; }

        public string? NewPasswordConfirm { get; set; }

        public bool Locked { get; set; }

        public bool Otp { get; set; }

        public int? OverrideMinPasswordLength { get; set; }

        public int? OverridePasswordExpireDays { get; set; }

        public int? OverrideUppercaseCount { get; set; }

        public int? OverrideNumbersCount { get; set; }
    }
}
