using MerchApp.Models;
using System.Threading.Tasks;

namespace MerchApp.Services
{
    public interface IAuthService
    {
        AppUser? CurrentUser { get; }
        bool IsLoggedIn { get; }

        Task<AppUser> LoginAsync();
        Task<string> GetAccessTokenAsync();
        Task LogoutAsync();
    }
}
