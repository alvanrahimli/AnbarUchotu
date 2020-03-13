using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AnbarUchotu.Utilities;

namespace AnbarUchotu.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Guid { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string NormalizedUsername { get; set; }
        [Required]
        [EmailAddress]
        public string NormalizedEmail { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        public string Role { get; set; }
    }
}