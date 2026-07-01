using MerchApp.Models;
using System;

namespace MerchApp.Services.Interfaces
{
    public interface ISessionContext
    {
        AppUser? CurrentUser { get; }
        bool IsLoggedIn { get; }
        bool IsManager { get; }
        event EventHandler? UserChanged;
        void SetUser(AppUser user);
        void ClearUser();
    }
}
