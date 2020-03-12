using System;
using System.Collections.Generic;
using AnbarUchotu.Utilities;

namespace AnbarUchotu.Models.Dtos
{
    public class TransactionReturnDto
    {
        public string Guid { get; set; }
        public decimal Amount { get; set; }
        public string IssuerGuid { get; set; }
        public ICollection<SoldProductReturnDto> Content { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public TransactionStatus Status { get; set; }
    }
}