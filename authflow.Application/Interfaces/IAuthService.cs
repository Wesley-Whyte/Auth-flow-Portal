using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authflow.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<string?> LoginAsync(string usernameOrEmail, string password);

        public Task<string> RegisterAsync(string username, string email, string password);
    }
}
