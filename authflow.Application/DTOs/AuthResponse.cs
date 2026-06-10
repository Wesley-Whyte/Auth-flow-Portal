using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authflow.Application.DTOs
{
    public record AuthResponse
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}