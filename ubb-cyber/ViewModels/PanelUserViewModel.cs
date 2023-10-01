namespace ubb_cyber.ViewModels
{
    public class PanelUserViewModel
    {
        public int Id { get; set; }

        public required string Login { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool Locked { get; set; }
    }
}
