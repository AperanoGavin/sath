using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using PRS.Domain.Repositories;
using PRS.Domain.Specifications;
using PRS.Infrastructure.EF.Contexts;
using PRS.Infrastructure.EF.Initialization;
using PRS.Infrastructure.EF.Repositories;
using PRS.Infrastructure.EF.Specifications;

namespace PRS.Infrastructure.EF;

internal static class ServiceCollectionExtensions
{

    public static IServiceCollection AddEF(this IServiceCollection services)
    {
        return
            services
                .AddDbContext<AppDbContext>(static options => options.UseInMemoryDatabase(databaseName: "prs_db"))
                .AddScoped<ISpotRepository, EFSpotRepository>()
                .AddScoped<IReservationRepository, EFReservationRepository>()
                .AddScoped<IUserReadOnlyRepository, EFUserRepository>()
                .AddScoped<IUnitOfWork, EFUnitOfWork>()
                .AddScoped<ISpotKeyUniquenessSpec, EFSpotKeyUniquenessSpec>()
                .AddScoped<IReservationOverlapSpec, EFReservationOverlapSpec>()
                .AddHostedService<DataSeederHostedService>();
        ;
    }
}