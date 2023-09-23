using System.ComponentModel.DataAnnotations;

namespace ubb_cyber.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(64)]
        public required string Login { get; set; }

        [MaxLength(256)]
        public required string PasswordHash { get; set; }

        public DateTime? LastLogin { get; set; }
    }
}
