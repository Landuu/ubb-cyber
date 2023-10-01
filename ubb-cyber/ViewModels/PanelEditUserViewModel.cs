namespace ubb_cyber.ViewModels
{
    public class PanelEditUserViewModel
    {
        public int Id { get; set; }

        public required string Login { get; set; }

        public string? NewPassword { get; set; }

        public string? NewPasswordConfirm { get; set; }

        public bool Locked { get; set; }
    }
}
