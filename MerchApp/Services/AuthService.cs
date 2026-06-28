using MerchApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MerchApp.Services
{
    public class AuthService : IAuthService
    {
        public AppUser? CurrentUser => throw new NotImplementedException();

        public bool IsLoggedIn => throw new NotImplementedException();

        public Task<string> GetAccessTokenAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AppUser> LoginAsync()
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
