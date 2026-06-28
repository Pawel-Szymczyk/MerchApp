using MerchApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MerchApp.Services
{
    public class SharePointService : ISharePointService
    {
        public Task<int> AddItemAsync(string title, int totalCount, string description = "")
        {
            throw new NotImplementedException();
        }

        public Task ApproveRequestAsync(int requestId, string note = "")
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateRentalRequestAsync(AppUser user, List<CartItem> cartItems, DateTime rentalFrom, DateTime rentalTo, string purpose = "")
        {
            throw new NotImplementedException();
        }

        public Task DeleteItemAsync(int itemId)
        {
            throw new NotImplementedException();
        }

        public Task<List<RentalRequest>> GetAllRentalRequestsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DashboardStats> GetDashboardStatsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Item>> GetItemsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<RentalRequest>> GetMyRentalRequestsAsync(string userEmail)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsReturnedAsync(int requestId)
        {
            throw new NotImplementedException();
        }

        public Task RejectRequestAsync(int requestId, string reason)
        {
            throw new NotImplementedException();
        }

        public Task<int> TestConnectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateItemCountAsync(int itemId, int totalCount)
        {
            throw new NotImplementedException();
        }
    }
}
