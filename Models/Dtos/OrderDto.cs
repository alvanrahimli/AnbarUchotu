using System.ComponentModel.DataAnnotations;

namespace AnbarUchotu.Models.Dtos
{
    public class OrderDto
    {
        [Required]
        public string ProductGuid { get; set; }
        public int Count { get; set; }
    }
}