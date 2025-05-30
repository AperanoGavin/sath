using Microsoft.Extensions.DependencyInjection;

using PRS.Infrastructure.EF;

namespace PRS.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return
            services
                .AddEF()
            ;
    }
}