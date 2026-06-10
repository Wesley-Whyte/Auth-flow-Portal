using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using authflow.Application.Interfaces;

namespace authflow.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepo _authRepo;

        public AuthService(IAuthRepo authRepo)
        {
            _authRepo = authRepo;
        }

        public async Task<string?> LoginAsync(string usernameOrEmail, string password)
        {
            return await _authRepo.LoginAsync(usernameOrEmail, password);
        }

        public async Task<string> RegisterAsync(string username, string email, string password)
        {
            return await _authRepo.RegisterAsync(username, email, password);
        }
    }
}
