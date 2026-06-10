using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using authflow.Application;
using authflow.Application.Interfaces;
using authflow.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace authflow.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplication();
            services.AddHttpClient("AuthApi", client =>
            {
                client.BaseAddress = new Uri(configuration["AuthApi:BaseUrl"] ?? throw new InvalidOperationException("AuthApi:BaseUrl is not configured"));
            });
            services.AddScoped<IAuthRepo, AuthRepo>();
        }
    }
}