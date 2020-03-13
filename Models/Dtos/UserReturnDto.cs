using AnbarUchotu.Utilities;

namespace AnbarUchotu.Models.Dtos
{
    public class UserReturnDto
    {
        public string Guid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}