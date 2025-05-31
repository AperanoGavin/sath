using Microsoft.EntityFrameworkCore;

using PRS.Infrastructure.EF.Contexts;

namespace PRS.Infrastructure.Tests.EF.Contexts;

internal static class InMemoryContextFactory
{
    public static AppDbContext Create(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        return new AppDbContext(options);
    }
}