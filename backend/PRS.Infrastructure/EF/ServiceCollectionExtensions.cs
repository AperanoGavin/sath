using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using PRS.Infrastructure.EF.Contexts;

namespace PRS.Infrastructure.EF;

internal static class ServiceCollectionExtensions
{

    public static IServiceCollection AddEF(this IServiceCollection services)
    {
        return
            services
                .AddDbContext<AppDbContext>(static options => options.UseInMemoryDatabase(databaseName: "prs_db"))
            ;
    }
}