using CarApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CarApp.Services {
    public class DemoDataService {

        private readonly AppDbContext _context;

        public DemoDataService(AppDbContext context) {
            _context = context;
        }

        public async Task SeedDemoDataAsync(string newUserId) {
            // Najdi šablonového uživatele "tryMe"
            var templateUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "tryMe");
            if (templateUser == null) return;

            // Najdi všechna auta tohoto uživatele
            var templateCars = await _context.Cars
                .Where(c => c.UserID == templateUser.Id)
                .ToListAsync();

            // Mapa starých ID aut -> nových ID
            var carIdMap = new Dictionary<int, int>();

            // Klonování aut
            foreach (var oldCar in templateCars) {
                var newCar = new Car {
                    Brand = oldCar.Brand,
                    Model = oldCar.Model,
                    Year = oldCar.Year,
                    Color = oldCar.Color,
                    LicensePlate = oldCar.LicensePlate,
                    Mileage = oldCar.Mileage,
                    Fuel = oldCar.Fuel,
                    NextMOT = oldCar.NextMOT,
                    UserID = newUserId
                };

                _context.Cars.Add(newCar);
                await _context.SaveChangesAsync();

                carIdMap[oldCar.Id] = newCar.Id;
            }

            // Klonování oprav
            var templateRepairs = await _context.Repairs
                .Where(r => r.UserID == templateUser.Id)
                .ToListAsync();

            foreach (var oldRepair in templateRepairs) {
                if (!carIdMap.TryGetValue(oldRepair.CarId, out var newCarId))
                    continue;

                var newRepair = new Repair {
                    Description = oldRepair.Description,
                    RepairDateStart = oldRepair.RepairDateStart,
                    RepairDateEnd = oldRepair.RepairDateEnd,
                    DaysInService = oldRepair.DaysInService,
                    MileageAtRepair = oldRepair.MileageAtRepair,
                    Cost = oldRepair.Cost,
                    CarId = newCarId,
                    UserID = newUserId
                };

                _context.Repairs.Add(newRepair);
            }

            // Klonování jízd
            var templateTrips = await _context.TripLogs
                .Where(t => t.UserID == templateUser.Id)
                .ToListAsync();

            foreach (var oldTrip in templateTrips) {
                if (!carIdMap.TryGetValue(oldTrip.CarId, out var newCarId))
                    continue;

                var newTrip = new TripLog {
                    StartDate = oldTrip.StartDate,
                    EndDate = oldTrip.EndDate,
                    DaysOut = oldTrip.DaysOut,
                    DistanceKm = oldTrip.DistanceKm,
                    Purpose = oldTrip.Purpose,
                    CarId = newCarId,
                    UserID = newUserId
                };

                _context.TripLogs.Add(newTrip);
            }
            await _context.SaveChangesAsync();
        }


        public async Task CleanupDemoDataAsync(string userId) {
            var repairs = _context.Repairs.Where(r => r.UserID == userId);
            var trips = _context.TripLogs.Where(t => t.UserID == userId);
            var cars = _context.Cars.Where(c => c.UserID == userId);

            _context.Repairs.RemoveRange(repairs);
            _context.TripLogs.RemoveRange(trips);
            _context.Cars.RemoveRange(cars);

            await _context.SaveChangesAsync();
        }

    }
}
