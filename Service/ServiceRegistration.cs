using Data.Dtos.Customer;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Services;
using Service.Services.Cache;
using Service.Services.Cache.Interfaces;
using Service.Services.Interfaces;
using Service.Validations.FluentValidation;

namespace Service
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IAppUserService, AppUserService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IRedisCacheService, RedisCacheService>();

            services.AddScoped<IValidator<AddCustomerRequestParameters>, CustomerAddValidator>();
            services.AddScoped<IValidator<UpdateCustomerRequestParameters>, CustomerUpdateValidator>();

            return services;
        }
    }
}
