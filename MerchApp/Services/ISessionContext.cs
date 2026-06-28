using MerchApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp.Services
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
