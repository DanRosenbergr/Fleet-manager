using CarApp.DTO;
using CarApp.Models;
using CarApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace CarApp.Controllers {
    [Authorize]
    public class CarsController : Controller {

        CarService _carServices;
        RepairService _repairService;
        TriplogService _tripLogService;

        public CarsController(CarService carServices, RepairService repairService, TriplogService tripLogService) {
            _carServices = carServices;
            _repairService = repairService;
            _tripLogService = tripLogService;
        }


        public IActionResult Index() {
            var allCars = _carServices.GetAllCars();
            ViewBag.FuelTypes = new SelectList(Enum.GetValues(typeof(FuelType)));
            ViewBag.SortOptions = new SelectList(Enum.GetValues(typeof(SortOptions)));
            return View(allCars);
        }

        [HttpGet]
        public IActionResult Create() {
            ViewBag.FuelTypes = new SelectList(Enum.GetValues(typeof(FuelType)));            
            return View();            
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarDTO newCar) {
            await _carServices.CreateCarAsync(newCar);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id) {
            await _carServices.DeleteCarAsync(id);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            ViewBag.FuelTypes = new SelectList(Enum.GetValues(typeof(FuelType)));
            var carToEdit = await _carServices.GetByIdAsync(id);
            return View(carToEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CarDTO carDto) {
            //if (!ModelState.IsValid) {
            //    ViewBag.FuelTypes = new SelectList(Enum.GetValues(typeof(FuelType)));
            //    return View(carDto); 
            //}
            await _carServices.UpdateAsync(carDto);
            return RedirectToAction("Index"); ;
        }

        public async Task<IActionResult> Details(int id) {
            var car = await _carServices.GetByIdAsync(id);
            if (car == null)
                return NotFound();

            // Získání oprav a triplogů pro dané auto
            var repairs = await _repairService.GetAllRepairs(id);
            var tripLogs = await _tripLogService.GetAllTripLogs(id);

            // Naplnění seznamů
            car.Repairs = repairs;
            car.TripLogs = tripLogs;

            return View(car);
        }
        // search by Brand or Name
        public IActionResult Search(string search) {
            if (string.IsNullOrEmpty(search)) {
                return RedirectToAction("Index");
            }
            ViewBag.FuelTypes = new SelectList(Enum.GetValues(typeof(FuelType)));
            ViewBag.SortOptions = new SelectList(Enum.GetValues(typeof(SortOptions)));
            var searchResults = _carServices.GetByName(search);
            return View("Index", searchResults);
        }

        //Search by fuel
        public IActionResult SearchByFuel(FuelType? fuel) {
            if (fuel == null) {
                return RedirectToAction("Index");
            }

            ViewBag.FuelTypes = new SelectList(Enum.GetValues(typeof(FuelType)), fuel);
            ViewBag.SortOptions = new SelectList(Enum.GetValues(typeof(SortOptions)));
            var cars = _carServices.GetByFuel(fuel.Value);
            return View("Index", cars);
        }

        //Order by selection
        public IActionResult OrderBy(SortOptions sortOption, string sortDirection) {
            bool descending = sortDirection == "Desc";
            var orderedCars = _carServices.OrderBy(sortOption, descending);
            ViewBag.FuelTypes = new SelectList(Enum.GetValues(typeof(FuelType)));
            ViewBag.SortOptions = new SelectList(Enum.GetValues(typeof(SortOptions)), sortOption);
            ViewBag.SortDirection = sortDirection; 
            return View("Index", orderedCars);         
        }
    }
}
