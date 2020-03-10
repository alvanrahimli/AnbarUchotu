using System.ComponentModel.DataAnnotations;

namespace AnbarUchotu.Models.Dtos
{
    public class UserLoginDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }

        public bool IsEmail
        {
            get { return Password.Contains('@'); }
        }
    }
}