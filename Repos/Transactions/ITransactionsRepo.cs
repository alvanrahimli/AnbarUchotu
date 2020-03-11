using System.Collections.Generic;
using System.Threading.Tasks;
using AnbarUchotu.Models.Dtos;

namespace AnbarUchotu.Repos.Transactions
{
    public interface ITransactionsRepo
    {
        Task<(TransactionReturnDto transaction, List<ProductReturnDto> unableProducts)> Create(string uGuid, List<ProductBuyDto> wantedProducts);
        Task<TransactionReturnDto> GetTransaction(string Guid);
        Task<bool> Sign(string tGuid);
        Task<bool> CancelTransaction(string tGuid);
    }
}