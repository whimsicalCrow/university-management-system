using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using University.Domain.Repositories;
using University.Infrastructure.Persistence;
using University.Infrastructure.Repositories;

namespace University.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UniversityDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("UniversityDatabase")
                ?? throw new InvalidOperationException("Connection string 'UniversityDatabase' was not found.");

            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IThesisProjectRepository, ThesisProjectRepository>();

        return services;
    }
}
