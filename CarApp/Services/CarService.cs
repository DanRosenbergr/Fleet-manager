using System.Globalization;
using CarApp.DTO;
using CarApp.Models;
using CarApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarApp.Services {
    public class CarService {

        AppDbContext _dbContext;
        UserManager<AppUser> _userManager;
        IHttpContextAccessor _httpContextAccessor;

        public CarService(AppDbContext context, UserManager<AppUser> userManager, 
            IHttpContextAccessor httpContextAccessor) 
            {
            _dbContext = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<CarDTO> GetAllCars() {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var allCars = _dbContext.Cars.Where(u => u.UserID == userId).ToList();
            var carDtos = new List<CarDTO>();
            foreach (var car in allCars) {
                carDtos.Add(new CarDTO {
                    Id = car.Id,
                    Brand = car.Brand,
                    Model = car.Model,
                    Year = car.Year,
                    Color = car.Color,
                    LicensePlate = car.LicensePlate,
                    Mileage = car.Mileage,
                    Fuel = car.Fuel,
                    NextMOT = car.NextMOT
                });
            }
            return carDtos;
        }

        internal async Task CreateCarAsync(CarDTO newCar) {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

            var carToSave = new Car {
                Id = newCar.Id,
                Brand = newCar.Brand,
                Model = newCar.Model,
                Year = newCar.Year,
                Color = newCar.Color,
                LicensePlate = newCar.LicensePlate,
                Mileage = newCar.Mileage,
                Fuel = newCar.Fuel,
                NextMOT = newCar.NextMOT,
                UserID = userId
            };
            _dbContext.Cars.Add(carToSave);
            await _dbContext.SaveChangesAsync();
        }

        internal async Task DeleteCarAsync(int id) {
            var carToDelete = await _dbContext.Cars.FindAsync(id);
            if (carToDelete != null) {
                _dbContext.Cars.Remove(carToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

        internal async Task<CarDTO> GetByIdAsync(int id) {
            var carToEdit = await _dbContext.Cars.FindAsync(id);
            if (carToEdit == null) return null;
            return new CarDTO {
                Id = carToEdit.Id,
                Brand = carToEdit.Brand,
                Model = carToEdit.Model,
                Year = carToEdit.Year,
                Color = carToEdit.Color,
                LicensePlate = carToEdit.LicensePlate,
                Mileage = carToEdit.Mileage,
                Fuel = carToEdit.Fuel,
                NextMOT = carToEdit.NextMOT
            };
        }             

        internal async Task UpdateAsync(CarDTO carDTO) {
            var carToUpdate = await _dbContext.Cars.FindAsync(carDTO.Id);
            if (carToUpdate == null) {
                // Např. throw nebo vrátit null, nebo cokoli dle tvé logiky
                return;
            }           
            carToUpdate.Brand = carDTO.Brand;
            carToUpdate.Model = carDTO.Model;
            carToUpdate.Year = carDTO.Year;
            carToUpdate.Color = carDTO.Color;
            carToUpdate.LicensePlate = carDTO.LicensePlate;
            carToUpdate.Mileage = carDTO.Mileage;
            carToUpdate.Fuel = carDTO.Fuel;
            carToUpdate.NextMOT = carDTO.NextMOT;          

            await _dbContext.SaveChangesAsync();
        }
        // search by brand controller
        internal IEnumerable<CarDTO> GetByName(string search) {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var searchResults = _dbContext.Cars.Where(u => u.UserID == userId)
                .Where(c => c.Brand.Contains(search) || c.Model.Contains(search))
                .ToList();
          
            var carDtos = new List<CarDTO>();
            foreach (var car in searchResults) {
                carDtos.Add(new CarDTO {
                    Id = car.Id,
                    Brand = car.Brand,
                    Model = car.Model,
                    Year = car.Year,
                    Color = car.Color,
                    LicensePlate = car.LicensePlate,
                    Mileage = car.Mileage,
                    Fuel = car.Fuel,
                    NextMOT = car.NextMOT
                });
            }
            return carDtos;

        }
        // search by fuel controller
        internal IEnumerable<CarDTO> GetByFuel(FuelType fuel) {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            return _dbContext.Cars.Where(u => u.UserID == userId)
                .Where(c => c.Fuel == fuel)
                .Select(c => new CarDTO {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    Color = c.Color,
                    LicensePlate = c.LicensePlate,
                    Mileage = c.Mileage,
                    Fuel = c.Fuel,
                    NextMOT = c.NextMOT
                })
                .ToList();
        }

        internal IEnumerable<CarDTO> OrderBy(SortOptions sortOption, bool descending) {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var query = _dbContext.Cars.Where(u => u.UserID == userId).AsQueryable();

            switch (sortOption) {
                case SortOptions.Brand:
                    query = descending ? query.OrderByDescending(c => c.Brand) : query.OrderBy(c => c.Brand);
                    break;              
                case SortOptions.Year:
                    query = descending ? query.OrderByDescending(c => c.Year) : query.OrderBy(c => c.Year);
                    break;
                case SortOptions.Mileage:
                    query = descending ? query.OrderByDescending(c => c.Mileage) : query.OrderBy(c => c.Mileage);
                    break;
                case SortOptions.NextMOT:
                    query = descending ? query.OrderByDescending(c => c.NextMOT) : query.OrderBy(c => c.NextMOT);
                    break;
                default:
                    break;
            }

            return query.Select(c => new CarDTO {
                Id = c.Id,
                Brand = c.Brand,
                Model = c.Model,
                Year = c.Year,
                Color = c.Color,
                LicensePlate = c.LicensePlate,
                Mileage = c.Mileage,
                Fuel = c.Fuel,
                NextMOT = c.NextMOT
            }).ToList();
        }

       
    }
}
