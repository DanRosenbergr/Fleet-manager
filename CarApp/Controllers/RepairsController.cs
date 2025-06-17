using CarApp.DTO;
using CarApp.Services;
using CarApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarApp.Controllers {
    [Authorize]
    public class RepairsController : Controller {

        RepairService _repairService;

        public RepairsController(RepairService repairService) {
            _repairService = repairService;
        }

        public IActionResult Index() {
            var allrepairs = _repairService.GetAllRepairs();

            ViewBag.SortOptionsRepairs = new SelectList(new List<SelectListItem> {
                new SelectListItem { Value = "RepairDateStart", Text = "Start Date" },
                new SelectListItem { Value = "RepairDateEnd", Text = "End Date" },
                new SelectListItem { Value = "Cost", Text = "Cost" },                
                }, "Value", "Text");

            return View(allrepairs);
        }

        [HttpGet]
        public IActionResult Create(int carId) {            
            var repairDto = new RepairDTO { CarId = carId };
            return View(repairDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RepairDTO newRepair) {
            await _repairService.CreateRepairAsync(newRepair);            
            return RedirectToAction("Details", "Cars", new { id = newRepair.CarId});
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id) {
            var carId = await _repairService.DeleteRepairAsync(id);
            if (carId == null)
                return NotFound();
            return RedirectToAction("Details", "Cars", new { id = carId });
        }

        [HttpGet]
        public async Task<IActionResult> EditAsync(int id) {
            var repairToEdit = await _repairService.GetByIdAsync(id);
            return View(repairToEdit);
        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(RepairDTO repairDTO, int id) {
            await _repairService.UpdateAsync(repairDTO, repairDTO.Id);
            return RedirectToAction("Details", "Cars", new { id = repairDTO.CarId });
        }
        //Order by selection
        public IActionResult OrderBy(sortOptionRepairs sortOptionRepair, string sortDirection) {
            bool descending = sortDirection == "Desc";
            var orderedRepairs = _repairService.OrderBy(sortOptionRepair, descending);

            ViewBag.SortOptionsRepairs = new SelectList(new List<SelectListItem> {
                new SelectListItem { Value = "RepairDateStart", Text = "Start Date" },
                new SelectListItem { Value = "RepairDateEnd", Text = "End Date" },
                new SelectListItem { Value = "Cost", Text = "Cost" },               
                }, "Value", "Text", sortOptionRepair);

            ViewBag.sortDirection = sortDirection;
            return View("Index", orderedRepairs);
        }
    }
}
