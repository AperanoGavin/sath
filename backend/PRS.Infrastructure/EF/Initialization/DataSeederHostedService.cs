using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PRS.Infrastructure.EF.Contexts;

namespace PRS.Infrastructure.EF.Initialization
{
    internal class DataSeederHostedService(IServiceProvider provider) : IHostedService
    {
        private readonly IServiceProvider _provider = provider;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: Make this configurable, we don't always want this behavior to occur
            // e.g: Dev env vs Prod, etc...

            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            DbInitializer.Initialize(db);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}