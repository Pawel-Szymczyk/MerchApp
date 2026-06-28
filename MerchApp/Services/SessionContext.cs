using MerchApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp.Services
{
    public class SessionContext : ISessionContext
    {
        public AppUser? CurrentUser => throw new NotImplementedException();

        public bool IsLoggedIn => throw new NotImplementedException();

        public bool IsManager => throw new NotImplementedException();

        public event EventHandler? UserChanged;

        public void ClearUser()
        {
            throw new NotImplementedException();
        }

        public void SetUser(AppUser user)
        {
            throw new NotImplementedException();
        }
    }
}
