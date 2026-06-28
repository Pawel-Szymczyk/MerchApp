using MerchApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp.Services
{
    public class SessionContext : ISessionContext
    {
        public AppUser? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;
        public bool IsManager => CurrentUser?.IsManager ?? false;

        public event EventHandler? UserChanged;

        public void SetUser(AppUser user)
        {
            CurrentUser = user;
            UserChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ClearUser()
        {
            CurrentUser = null;
            UserChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
