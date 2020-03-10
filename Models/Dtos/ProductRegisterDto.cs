using System.ComponentModel.DataAnnotations;

namespace AnbarUchotu.Models.Dtos
{
    public class ProductRegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(250, MinimumLength = 3)]
        public string Description { get; set; }

        [Required]
        public string Barcode { get; set; }

        [Required]
        public decimal Mass { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Count { get; set; }
    }
}