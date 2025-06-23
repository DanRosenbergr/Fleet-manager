using CarApp.DTO;
using CarApp.Models;
using CarApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarApp.Services {
    public class RepairService {

        AppDbContext _dbContext;
        UserManager<AppUser> _userManager;
        IHttpContextAccessor _httpContextAccessor;

        public RepairService(AppDbContext dbcontext, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor) {
            _dbContext = dbcontext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        public IEnumerable<RepairDTO> GetAllRepairs() {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var allRepairs = _dbContext.Repairs.Where(u => u.UserID == userId).Include(r => r.Car).ToList();
            var repairDtos = new List<RepairDTO>();
            foreach (var repair in allRepairs) {
                RepairDTO repairDto = ModelToDto(repair);
                //pro ziskani dat pro partial repair table
                if (repair.Car != null) {
                    repairDto.CarBrand = repair.Car.Brand;
                    repairDto.CarModel = repair.Car.Model;
                }
                repairDtos.Add(repairDto);
            }
            return repairDtos;
        }

        public async Task<List<RepairDTO>> GetAllRepairs(int carId) {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var repairs = await _dbContext.Repairs.Where(u => u.UserID == userId).Include(r => r.Car).Where(r => r.CarId == carId).ToListAsync();
            var repairDtos = new List<RepairDTO>();
            foreach (var repair in repairs) {
                var repairDto = ModelToDto(repair);
                if (repair.Car != null) {
                    repairDto.CarBrand = repair.Car.Brand;
                    repairDto.CarModel = repair.Car.Model;
                }
                repairDtos.Add(repairDto);
            }
            return repairDtos;
        }

        internal async Task<int?> DeleteRepairAsync(int id) {
            var repairToDelete = await _dbContext.Repairs.FindAsync(id);
            if (repairToDelete == null) {
                return null;
            }
            int carId = repairToDelete.CarId;
            _dbContext.Repairs.Remove(repairToDelete);
            await _dbContext.SaveChangesAsync();
            return carId;
        }

        internal async Task CreateRepairAsync(RepairDTO repairDto) {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var car = await _dbContext.Cars.FindAsync(repairDto.CarId);
            if (car == null)
                throw new Exception("Car does not exists.");

            Repair newRepair = DtoToModel(repairDto);
            newRepair.UserID = userId;

            _dbContext.Repairs.Add(newRepair);

            await _dbContext.SaveChangesAsync();
        }
        
        //double edit metody
        internal async Task<RepairDTO> GetByIdAsync(int id) {
            var repairToEdit = await _dbContext.Repairs.FindAsync(id);
            if (repairToEdit == null)
                throw new Exception($"Repair with ID:{id} not found.");
            return ModelToDto(repairToEdit);
        }
        internal async Task UpdateAsync(RepairDTO repairDto, int id) {
            var repairToUpdate = await _dbContext.Repairs.FindAsync(id);
            if (repairToUpdate == null)
                throw new Exception($"Repair with ID:{id} not found.");
            // Aktualizace vlastností
            repairToUpdate.Description = repairDto.Description;
            repairToUpdate.RepairDateStart = repairDto.RepairDateStart;
            repairToUpdate.RepairDateEnd = repairDto.RepairDateEnd;
            repairToUpdate.DaysInService = (repairDto.RepairDateEnd.DayNumber - repairDto.RepairDateStart.DayNumber + 1);
            repairToUpdate.MileageAtRepair = repairDto.MileageAtRepair;
            repairToUpdate.Cost = repairDto.Cost;
            repairToUpdate.CarId = repairDto.CarId;
            await _dbContext.SaveChangesAsync();
        }           

        internal IEnumerable<RepairDTO> OrderBy(sortOptionRepairs sortOption, bool descending) {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var query = _dbContext.Repairs.Where(u => u.UserID == userId).Include(r => r.Car).AsQueryable();

            switch (sortOption) {
                case sortOptionRepairs.RepairDateStart:
                    query = descending ? query.OrderByDescending(r => r.RepairDateStart) : query.OrderBy(r => r.RepairDateStart);
                    break;
                case sortOptionRepairs.RepairDateEnd:
                    query = descending ? query.OrderByDescending(r => r.RepairDateEnd) : query.OrderBy(r => r.RepairDateEnd);
                    break;
                case sortOptionRepairs.Cost:
                    query = descending ? query.OrderByDescending(r => r.Cost) : query.OrderBy(r => r.Cost);
                    break;
                case sortOptionRepairs.DaysInService:
                    query = descending ? query.OrderByDescending(r => r.DaysInService) : query.OrderBy(r => r.DaysInService);
                    break;
                default:
                    break;
            }

            var repairDtos = new List<RepairDTO>();
            foreach (var repair in query.ToList()) {
                var dto = ModelToDto(repair);
                if (repair.Car != null) {
                    dto.CarBrand = repair.Car.Brand;
                    dto.CarModel = repair.Car.Model;
                }
                repairDtos.Add(dto);
            }
            return repairDtos;
        }

        //pomocne metody
        public RepairStatsViewModel BuildRepairStats(List<RepairDTO> repairs) {
            var totalCost = repairs.Sum(r => r.Cost);
            var averageCost = repairs.Any() ? repairs.Average(r => r.Cost) : 0;
            var totalDaysOff = repairs.Sum(d => d.DaysInService);

            var highcost = repairs.GroupBy(c => c.CarId)
                            .OrderByDescending(g => g.Sum(r => r.Cost))
                            .Select(g => new {
                                CarId = g.Key,
                                highestCost = g.Sum(r => r.Cost),
                                CarBrand = g.First().CarBrand,
                                CarModel = g.First().CarModel
                            })
                            .FirstOrDefault();

            var longestOff = repairs.GroupBy(c => c.CarId)
                            .OrderByDescending(g => g.Sum(r => r.DaysInService))
                            .Select(g => new {
                                CarId = g.Key,
                                DaysOff = g.Sum(r => r.DaysInService),
                                CarBrand = g.First().CarBrand,
                                CarModel = g.First().CarModel
                            })
                            .FirstOrDefault();

            var mostOften = repairs.GroupBy(r => r.CarId)
                            .OrderByDescending(g => g.Count())
                            .Select(g => new {
                                CarId = g.Key,
                                Count = g.Count(),
                                CarBrand = g.First().CarBrand,
                                CarModel = g.First().CarModel
                            })
                            .FirstOrDefault();

            return new RepairStatsViewModel {
                Repairs = repairs,
                TotalRepairs = repairs.Count,
                TotalCost = totalCost,
                AverageCost = averageCost,
                DaysOff = totalDaysOff,
                HighCost = highcost?.highestCost ?? 0,
                HighCostCarBrand = highcost?.CarBrand,
                HighCostCarModel = highcost?.CarModel,
                LongestOff = longestOff?.DaysOff ?? 0,
                LongestOffCarBrand = longestOff?.CarBrand,
                LongestOffCarModel = longestOff?.CarModel,
                MostRepairedCount = mostOften?.Count ?? 0,
                MostRepairedCarBrand = mostOften?.CarBrand,
                MostRepairedCarModel = mostOften?.CarModel
            };
        }
               
        private RepairDTO ModelToDto(Repair repair) {
            return new RepairDTO {
                Id = repair.Id,
                Description = repair.Description,
                RepairDateStart = repair.RepairDateStart,
                RepairDateEnd = repair.RepairDateEnd,
                DaysInService = repair.DaysInService,
                MileageAtRepair = repair.MileageAtRepair,
                Cost = repair.Cost,
                CarId = repair.CarId
            };
        }
        private Repair DtoToModel(RepairDTO repairDto) {
            return new Repair {
                Id = repairDto.Id,
                Description = repairDto.Description,
                RepairDateStart = repairDto.RepairDateStart,
                RepairDateEnd = repairDto.RepairDateEnd,
                DaysInService = (repairDto.RepairDateEnd.DayNumber - repairDto.RepairDateStart.DayNumber + 1),
                MileageAtRepair = repairDto.MileageAtRepair,
                Cost = repairDto.Cost,
                CarId = repairDto.CarId
            };
        }
    }
}
