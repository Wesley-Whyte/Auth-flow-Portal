using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using authflow.Application.DTOs;
using authflow.Application.Interfaces;


namespace authflow.Infrastructure.Repositories
{
    public class AuthRepo : IAuthRepo
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthRepo(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> LoginAsync(string username, string password)
        {
            var client = _httpClientFactory.CreateClient("AuthApi");
            var payload = new { username, password };
            var response = await client.PostAsJsonAsync("account/login", payload);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Login failed with status code: " + response.StatusCode);
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (result?.Token is null)
            {
                throw new Exception("Authentication failed. No token received.");
            }
            return result.Token;
        }

        public async Task<string> RegisterAsync(string username, string email, string password)
        {
            var client = _httpClientFactory.CreateClient("AuthApi");
            var payload = new { username, email, password };
            var response = await client.PostAsJsonAsync("account/register", payload);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Login failed with status code: " + response.StatusCode);
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (result?.Token is null)
            {
                throw new Exception("Authentication failed. No token received.");
            }
            return result.Token;
        }
    }
}
