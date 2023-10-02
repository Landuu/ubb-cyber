using System.ComponentModel.DataAnnotations;

namespace ubb_cyber.Models
{
    public class UsedPassword
    {
        [Key]
        public int Id { get; set; }

        public User User { get; set; } = null!;
        public int UserId { get; set; }

        [MaxLength(256)]
        public required string PasswordHash { get; set; }

        public DateTime ChangeDate { get; set; }
    }
}
