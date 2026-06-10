using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authflow.Application.Interfaces
{
    public interface IAuthRepo
    {
        public Task<string> LoginAsync(string username, string password);

        public Task<string> RegisterAsync(string username, string email, string password);
    }
}
