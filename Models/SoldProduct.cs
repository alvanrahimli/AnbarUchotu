using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnbarUchotu.Models
{
    public class SoldProduct
    {
        [Key]
        public string Guid { get; set; }

        [ForeignKey("Product")]
        public string ProductGuid { get; set; }

        public Product Product { get; set; }

        public int SoldCount { get; set; }

        [ForeignKey("Transaction")]
        public string TransactionGuid { get; set; }
        public Transaction Transaction { get; set; }
    }
}