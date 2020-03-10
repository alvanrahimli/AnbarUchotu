namespace AnbarUchotu.Models.Dtos
{
    public class CardReturnDto
    {
        public string Guid { get; set; }
        public string CardNumber { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal TotalCancelled { get; set; }
        public int IssuedTransactionsCount { get; set; }
        public int AcceptedTransactionsCount { get; set; }
        public string OwnerGuid { get; set; }
        public string OwnerName { get; set; }
    }
}