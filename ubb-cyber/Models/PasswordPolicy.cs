using System.ComponentModel.DataAnnotations;

namespace ubb_cyber.Models
{
    public class PasswordPolicy
    {
        [Key]
        public required string Key { get; set; }

        public int MinPasswordLength { get; set; }

        public int PasswordExpireDays { get; set; }

        public int UppercaseCount { get; set; }

        public int NumbersCount { get; set; }
    }
}
