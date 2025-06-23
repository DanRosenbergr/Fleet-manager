using CarApp.DTO;
using CarApp.Models;
using CarApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarApp.Services { 

    public class TriplogService {

        AppDbContext _dbContext;
        UserManager<AppUser> _userManager;
        IHttpContextAccessor _httpContextAccessor;

        public TriplogService(AppDbContext context, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor) {
            _dbContext = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<TripLogDTO> GetAllTrips() {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var alltrips = _dbContext.TripLogs.Where(u => u.UserID == userId).Include(c => c.Car).ToList();
            var tripDtos = new List<TripLogDTO>();
            foreach (var trip in alltrips) {
                TripLogDTO tripsDto = ModeltoDto(trip);
                tripDtos.Add(tripsDto);
                if (trip.Car != null) {
                    tripsDto.CarBrand = trip.Car.Brand;
                    tripsDto.CarModel = trip.Car.Model;
                }
            }
            return tripDtos;
        }

        public async Task<List<TripLogDTO>> GetAllTripLogs(int carId) {
            var tripLogs = await _dbContext.TripLogs
                .Where(t => t.CarId == carId)
                .ToListAsync();

            return tripLogs.Select(t => new TripLogDTO {
                Id = t.Id,
                CarId = t.CarId,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                DistanceKm = t.DistanceKm,
                Purpose = t.Purpose
            }).ToList();
        }

        internal async Task<int?> DeleteTripAsync(int id) {
            var tripToDelete = await _dbContext.TripLogs.FindAsync(id);
            if (tripToDelete == null) {
                return null; 
            }
            int carId = tripToDelete.CarId;

            _dbContext.TripLogs.Remove(tripToDelete);
            await _dbContext.SaveChangesAsync();
            return carId;
        }

        internal async Task CreateTripAsync(TripLogDTO tripLogDTO) {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var car = await _dbContext.Cars.FindAsync(tripLogDTO.CarId);
           
            if (car == null)
                throw new Exception("Car does not exists.");

            TripLog newTrip = DtoToModel(tripLogDTO);
            newTrip.UserID = userId; 
            _dbContext.TripLogs.Add(newTrip);

            // Přičtení distance k mileage auta
            car.Mileage += newTrip.DistanceKm;

            await _dbContext.SaveChangesAsync();
        }
        
        //double edit metoda
        internal async Task<TripLogDTO> GetByIdAsync(int id) {
            var tripToEdit = await _dbContext.TripLogs.FindAsync(id);
            if (tripToEdit == null)
                throw new Exception($"TripLog with ID:{id} not found.");
            return ModeltoDto(tripToEdit);
        }
        internal async Task UpdateAsync(TripLogDTO tripLogDTO, int id) { 
            
            var existingTrip = await _dbContext.TripLogs.FindAsync(id);
            if (existingTrip == null)
                throw new Exception($"TripLog with ID:{id} not found.");

            var car = await _dbContext.Cars.FindAsync(existingTrip.CarId);
            if (car == null)
                throw new Exception($"Car with ID:{existingTrip.CarId} not found.");
            
            int distanceDifference = tripLogDTO.DistanceKm - existingTrip.DistanceKm;
            car.Mileage += distanceDifference;

            existingTrip.StartDate = tripLogDTO.StartDate;
            existingTrip.EndDate = tripLogDTO.EndDate;
            existingTrip.DaysOut = (tripLogDTO.EndDate.DayNumber - tripLogDTO.StartDate.DayNumber + 1);
            existingTrip.DistanceKm = tripLogDTO.DistanceKm;
            existingTrip.Purpose = tripLogDTO.Purpose;
            existingTrip.CarId = tripLogDTO.CarId;

            await _dbContext.SaveChangesAsync();
        }              
       
        internal IEnumerable<TripLogDTO> OrderBy(SortOptionTrips sortOption, bool descending) {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var query = _dbContext.TripLogs.Where(u => u.UserID == userId).Include(r => r.Car).AsQueryable();

            switch (sortOption) {
                case SortOptionTrips.DistanceKm:
                    query = descending ? query.OrderByDescending(r => r.DistanceKm) : query.OrderBy(r => r.DistanceKm);
                    break;
                case SortOptionTrips.StartDate:
                    query = descending ? query.OrderByDescending(r => r.StartDate) : query.OrderBy(r => r.StartDate);
                    break;
                case SortOptionTrips.EndDate:
                    query = descending ? query.OrderByDescending(r => r.EndDate) : query.OrderBy(r => r.EndDate);
                    break;
                case SortOptionTrips.DaysOut:
                    query = descending ? query.OrderByDescending(r => r.DaysOut) : query.OrderBy(r => r.DaysOut);
                    break;
                default:
                    break;
            }

            var tripsDtos = new List<TripLogDTO>();
            foreach (var trip in query.ToList()) {
                var dto = ModeltoDto(trip);
                if (trip.Car != null) {
                    dto.CarBrand = trip.Car.Brand;
                    dto.CarModel = trip.Car.Model;
                }
                tripsDtos.Add(dto);
            }
            return tripsDtos;
        }

        //pomocne metody
        public TripLogStatsViewModel BuildTripsStats(List<TripLogDTO> tripLogs) {
            var TotalDistance = tripLogs.Sum(t => t.DistanceKm);
            var AverageDistanceKm = tripLogs.Count > 0 ? tripLogs.Average(t => t.DistanceKm) : 0;
            var AvdDaysOut = tripLogs.Count > 0 ? tripLogs.Average(t => t.DaysOut) : 0;

            var FurtherstTrip = tripLogs.OrderByDescending(t => t.DistanceKm).FirstOrDefault();

            var LongestTrip = tripLogs.OrderByDescending(r => r.DaysOut)
                            .Select(r => new {                               
                                DaysOff = r.DaysOut,
                                CarBrand = r.CarBrand,
                                CarModel = r.CarModel
                            })
                            .FirstOrDefault();
            var MostTripsCount = tripLogs.GroupBy(c => c.CarId)
                            .OrderByDescending(g => g.Count())
                            .Select(g => new {
                                CarId = g.Key,
                                TripsCount = g.Count(),
                                CarBrand = g.First().CarBrand,
                                CarModel = g.First().CarModel
                            })
                            .FirstOrDefault();

            return new TripLogStatsViewModel {
                TripLogs = tripLogs,
                SumTrips = tripLogs.Count,
                TotalDistance = TotalDistance,
                AvdDaysOut = AvdDaysOut,
                AvgDistance = AverageDistanceKm,
                FurtherstTrip = FurtherstTrip?.DistanceKm ?? 0,
                FurtherstTripCarBrand = FurtherstTrip?.CarBrand,
                FurtherstTripCarModel = FurtherstTrip?.CarModel,
                LongestTrip = LongestTrip?.DaysOff ?? 0,
                LongestTripCarBrand = LongestTrip?.CarBrand,
                LongestTripCarModel = LongestTrip?.CarModel,
                MostTripsCount = MostTripsCount?.TripsCount ?? 0,
                MostTripsCarBrand = MostTripsCount?.CarBrand,
                MostTripsCarModel = MostTripsCount?.CarModel,
            };
        }

        private TripLog DtoToModel(TripLogDTO triplogDto) {
            return new TripLog {
                Id = triplogDto.Id,
                StartDate = triplogDto.StartDate,
                EndDate = triplogDto.EndDate,
                DaysOut = (triplogDto.EndDate.DayNumber - triplogDto.StartDate.DayNumber + 1),
                DistanceKm = triplogDto.DistanceKm,
                Purpose = triplogDto.Purpose,
                CarId = triplogDto.CarId,
                UserID = triplogDto.UserID,
            };
        }
        private TripLogDTO ModeltoDto(TripLog triplog) {
            return new TripLogDTO {
                Id = triplog.Id,
                StartDate = triplog.StartDate,
                EndDate = triplog.EndDate,
                DaysOut = triplog.DaysOut,
                DistanceKm = triplog.DistanceKm,
                Purpose = triplog.Purpose,
                CarId = triplog.CarId,
                UserID = triplog.UserID,
            };
        }
    }
}
