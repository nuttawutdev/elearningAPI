using ELEARNING.Services.Interfaces;
using ELEARNING.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ELEARNING.Dependencies
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Logging
            services.AddScoped<ICourseService, CourseService>();

            return services;
        }

    }
}