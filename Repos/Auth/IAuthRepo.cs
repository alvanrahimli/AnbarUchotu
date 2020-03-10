using System.Threading.Tasks;
using AnbarUchotu.Models.Dtos;

namespace AnbarUchotu.Repos.Auth
{
    public interface IAuthRepo
    {
        Task<(UserReturnDto userCredentials, string token)> Login(UserLoginDto loginCreds);
        Task<(UserReturnDto userCredentials, string token)> Register(UserRegisterDto userCredentials);
    }
}