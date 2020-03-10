using System.Collections.Generic;
using System.Threading.Tasks;
using AnbarUchotu.Models.Dtos;

namespace AnbarUchotu.Repos.Products
{
    public interface IProductsRepo
    {
        Task<List<ProductReturnDto>> Products(int rn, int c);
        Task<ProductReturnDto> Product(string guid);
        Task<ProductReturnDto> Register(ProductRegisterDto product);
        Task<ProductReturnDto> Update(ProductUpdateDto product);
        Task<TransactionReturnDto> Buy(string uGuid, List<ProductBuyDto> products);
        Task<TransactionReturnDto> Retrieve(string tGuid);
    }
}