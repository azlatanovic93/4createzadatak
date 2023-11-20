using FourCreate.Domain.Entities.Employee.Factories;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FourCreate.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<IEmployeeFactory, EmployeeFactory>();

            return services;
        }
    }

}
