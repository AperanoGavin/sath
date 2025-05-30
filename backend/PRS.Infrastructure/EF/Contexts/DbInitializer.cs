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
                        spot.AddCapability(SpotCapability.ElectricCharger);
                    return spot;
                }))
            .ToList();
        context.Spots.AddRange(spots);


        if (!context.Roles.Any() && !context.Users.Any())
        {
            var employeeRole = new UserRole("Employee", "Employee", "Regular employee");
            var managerRole = new UserRole("Manager", "Manager", "Parking manager");
            context.Roles.AddRange(employeeRole, managerRole);

            var alice = User.Create("Alice", "alice@acme.com", employeeRole).Value;
            var bob = User.Create("Bob", "bob@acme.com", managerRole).Value;
            context.Users.AddRange(alice, bob);


            context.SaveChanges();


            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var r1 = Reservation.Create(
                        spots[0],
                        alice,
                        tomorrow,
                        tomorrow.AddDays(2)
                     ).Value;
            var r2 = Reservation.Create(
                        spots[1],
                        bob,
                        tomorrow,
                        tomorrow.AddDays(1)
                     ).Value;

            context.Reservations.AddRange(r1, r2);
        }

        context.SaveChanges();
    }
}