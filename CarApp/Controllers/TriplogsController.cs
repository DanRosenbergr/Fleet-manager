using CarApp.DTO;
using CarApp.Services;
using CarApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarApp.Controllers {
    [Authorize]
    public class TriplogsController : Controller {

        TriplogService _triplogServices;

        public TriplogsController(TriplogService triplogService) {
            _triplogServices = triplogService;            
        }

        public IActionResult Index() {
            
            ViewBag.SortOptionsTrips = new SelectList(new List<SelectListItem> {
                new SelectListItem { Value = "StartDate", Text = "Start Date" },
                new SelectListItem { Value = "EndDate", Text = "End Date" },
                new SelectListItem { Value = "DaysOut", Text = "Days out" },
                new SelectListItem { Value = "DistanceKm", Text = "Distance (km)" }
            }, "Value", "Text");

            var allTriplogs = _triplogServices.GetAllTrips().ToList();
            var model = _triplogServices.BuildTripsStats(allTriplogs);

            return View(model);         
        }

        [HttpGet]
        public IActionResult Create(int carId) {            
            var tripDto = new TripLogDTO { CarId = carId };
            return View(tripDto);
        }
        [HttpPost]
        public async Task<IActionResult> Create(TripLogDTO newTrip) {
            await _triplogServices.CreateTripAsync(newTrip);
            return RedirectToAction("Details", "Cars", new { id = newTrip.CarId});
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id) {
            var carId = await _triplogServices.DeleteTripAsync(id);
            if (carId == null)
                return NotFound();
            return RedirectToAction("Details", "Cars", new { id = carId });
        }
        

        [HttpGet]
        public async Task<IActionResult> EditAsync(int id) {
            var tripToEdit = await _triplogServices.GetByIdAsync(id);            
            return View(tripToEdit);            
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(TripLogDTO tripLogDTO, int id) {
            await _triplogServices.UpdateAsync(tripLogDTO, tripLogDTO.Id);

            return RedirectToAction("Details", "Cars", new { id = tripLogDTO.CarId });
        }

        //Order by selection
        public IActionResult OrderBy(SortOptionTrips sortOptionTrips, string sortDirection) {
            bool descending = sortDirection == "Desc";
            var orderedTrips = _triplogServices.OrderBy(sortOptionTrips, descending).ToList();

            ViewBag.SortOptionsTrips = new SelectList(new List<SelectListItem> {
                new SelectListItem { Value = "StartDate", Text = "Start Date" },
                new SelectListItem { Value = "EndDate", Text = "End Date" },
                new SelectListItem { Value = "DistanceKm", Text = "Distance" },               
                new SelectListItem { Value = "DaysOut", Text = "Days out" },               
                }, "Value", "Text", sortOptionTrips);

            ViewBag.SortDirection = sortDirection;
            var model = _triplogServices.BuildTripsStats(orderedTrips);

            return View("Index", model);       
        }
    }
}
