using PRS.Domain.Entities;
using PRS.Domain.Enums;

namespace PRS.Infrastructure.EF.Contexts;

internal static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Spots.Any()) return;

        var rows = new[] { 'A', 'B', 'C', 'D', 'E', 'F' };
        var spots = rows
            .SelectMany(row => Enumerable.Range(1, 10)
                .Select(i =>
                {

                    var key = $"{row}{i:00}";

                    var spot = Spot.Create(key).Value;


                    if (row is 'A' or 'F')
                    {
                        spot.AddCapability(SpotCapability.ElectricCharger);
                    }

                    return spot;
                })
            )
            .ToList();


        context.Spots.AddRange(spots);
        context.SaveChanges();
    }
}
