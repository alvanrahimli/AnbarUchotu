namespace AnbarUchotu.Models.Dtos
{
    public class ProductReturnDto
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        public decimal Mass { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
    }
}