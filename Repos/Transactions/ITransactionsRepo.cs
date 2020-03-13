using System.Collections.Generic;
using System.Threading.Tasks;
using AnbarUchotu.Models.Dtos;
using AnbarUchotu.Utilities;

namespace AnbarUchotu.Repos.Transactions
{
    public interface ITransactionsRepo
    {
        Task<(TransactionReturnDto transaction, List<ProductReturnDto> unableProducts)> Create(string uGuid, List<ProductBuyDto> wantedProducts);
        Task<TransactionReturnDto> GetTransaction(string Guid);
        Task<List<TransactionReturnDto>> GetTransactions(int rn, int c);
        Task<TransactionReturnDto> Sign(string tGuid);
        Task<List<TransactionReturnDto>> SignAll();
        Task<TransactionReturnDto> CancelTransaction(string tGuid);
        Task<List<TransactionReturnDto>> GetTransactionsByStatus(TransactionStatus status);
        Task<List<TransactionReturnDto>> GetForUser(string guid);
    }
}