using MerchApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerchApp.Services
{
    public interface ISharePointService
    {
        Task<int> TestConnectionAsync();
        Task<List<Models.Item>> GetItemsAsync();
        Task<int> AddItemAsync(string title, int totalCount, string description = "");
        Task DeleteItemAsync(int itemId);
        //Task UpdateItemCountAsync(int itemId, int totalCount);
        Task<int> CreateRentalRequestAsync(AppUser user, List<CartItem> cartItems, DateTime rentalFrom, DateTime rentalTo, string purpose = "");
        Task<List<RentalRequest>> GetAllRentalRequestsAsync();
        Task<List<RentalRequest>> GetMyRentalRequestsAsync(string userEmail);
        Task ApproveRequestAsync(int requestId, string note = "");
        Task RejectRequestAsync(int requestId, string reason);
        Task MarkAsReturnedAsync(int requestId);
        //Task<DashboardStats> GetDashboardStatsAsync();
    }
}
