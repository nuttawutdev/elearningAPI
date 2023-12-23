using ELEARNING.Repositories.Context;
using ELEARNING.Repositories.Interfaces;
using ELEARNING.Repositories.Repositories;
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
            services.AddScoped<ICourseService, CourseService>();

            services.AddSingleton<DBContext>();
            services.AddScoped<ICourseRepository, CourseRepository>();

            return services;
        }

    }
}