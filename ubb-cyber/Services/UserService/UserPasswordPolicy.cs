namespace ubb_cyber.Services.UserService
{
    public class UserPasswordPolicy
    {
        public required int MinPasswordLength { get; set; }

        public required int PasswordExpireDays { get; set; }

        public required int UppercaseCount { get; set; }

        public required int NumbersCount { get; set; }
    }
}
