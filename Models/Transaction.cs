using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AnbarUchotu.Utilities;

namespace AnbarUchotu.Models
{
    public class Transaction
    {
        [Key]
        public string Guid { get; set; }

        [ForeignKey("Issuer")]
        public string IssuerGuid { get; set; }
        public User Issuer { get; set; }
        public decimal Amount { get; set; }
        public ICollection<SoldProduct> Content { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public TransactionStatus Status { get; set; }
    }
}