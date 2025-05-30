using Microsoft.Extensions.DependencyInjection;

using PRS.Domain.Factories;
using PRS.Infrastructure;

namespace PRS.Application;
public static class ServiceCollectionExtensions
{
    private static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services
            .AddScoped<ISpotFactory, SpotFactory>()
            .AddScoped<IReservationFactory, ReservationFactory>();
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddDomain()
            .AddInfrastructure();

        services.AddMediatR(static cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        return services;
    }
}