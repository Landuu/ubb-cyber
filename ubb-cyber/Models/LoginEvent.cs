using System.ComponentModel.DataAnnotations;

namespace ubb_cyber.Models
{
    public class LoginEvent
    {
        [Key]
        public int Id { get; set; }

        public required DateTime InsertDate { get; set; }

        [MaxLength(128)]
        public required string UserLogin { get; set; }

        [MaxLength(256)]
        public required string Action { get; set; }

        [MaxLength(512)]
        public string? Description { get; set; }
    }
}
