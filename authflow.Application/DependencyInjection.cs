using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using authflow.Application.Interfaces;
using authflow.Application.Services;

using Microsoft.Extensions.DependencyInjection;

namespace authflow.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            // Register application services here, e.g.:
            // services.AddScoped<IMyService, MyService>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}