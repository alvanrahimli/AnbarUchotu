using System.ComponentModel.DataAnnotations;

namespace AnbarUchotu.Models.Dtos
{
    public class CardRegisterDto
    {
        [Required]
        public string CardNumber { get; set; }
        public decimal Balance { get; set; }
    }
}