using CarApp.Models;
using CarApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FleetManager.Controllers {
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller {

        UserManager<AppUser> _userManager;
        IPasswordHasher<AppUser> _passwordHasher;
        IPasswordValidator<AppUser> _passwordValidator;

        public UsersController(UserManager<AppUser> userManager, IPasswordHasher<AppUser> passwordHasher, IPasswordValidator<AppUser> passwordValidator) {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _passwordValidator = passwordValidator;


        }
        public async Task<IActionResult> Index() {
            var users = _userManager.Users.ToList();
            var model = new List<UsersViewModel>();

            foreach (var user in users) {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UsersViewModel {
                    Id = user.Id,
                    UserName = user.UserName,
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser() {           
            return View(new CreateUserViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model) {
            if (!ModelState.IsValid) {               
                return View(model);
            }
           
            var user = new AppUser {
                UserName = model.UserName,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded) {
                await _userManager.AddToRoleAsync(user, "User");
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors) {
                AddIdentityErrors(result);
            }
            
            return View(model);
        }

        //EDIT double metody
        [HttpGet]
        public async Task<IActionResult> EditUser(string id) {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
           
            var model = new EditUserViewModel {
                Id = user.Id,
                UserName = user.UserName,                
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model) {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            // Username change
            if (user.UserName != model.UserName) {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                if (!setUserNameResult.Succeeded) {
                    foreach (var error in setUserNameResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(model);
                }
                TempData["UserNameMessage"] = "Username was changed successfully.";
            }

            // Password change -only if NewPassword 
            if (!string.IsNullOrEmpty(model.NewPassword)) {
                if (string.IsNullOrEmpty(model.CurrentPassword)) {
                    ModelState.AddModelError("CurrentPassword", "Enter the current password.");
                    return View(model);
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded) {
                    foreach (var error in changePasswordResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(model);
                }
                TempData["Message"] = "Password updated successfully.";
            }
                       
            return RedirectToAction(nameof(EditUser), new { id = model.Id });            
        }


        [HttpPost]
        public async Task<IActionResult> DeleteAsync(string id) {
            AppUser userToDelete = await _userManager.FindByIdAsync(id);
            if (userToDelete != null) {
                IdentityResult result = await _userManager.DeleteAsync(userToDelete);
                if (result.Succeeded) {
                    return RedirectToAction("Index");
                } else {
                    AddIdentityErrors(result);
                }
            } else {
                ModelState.AddModelError("", "User not found");
            }
            return RedirectToAction("Index");
        }


        private void AddIdentityErrors(IdentityResult result) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError("", error.Description);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Search(string search) {
            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                users = users.Where(u => u.UserName.Contains(search));
            }

            var model = new List<UsersViewModel>();
            foreach (var user in users) {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UsersViewModel {
                    Id = user.Id,
                    UserName = user.UserName,
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }

            return View("Index", model); // znovu použiješ pohled Index.cshtml
        }

    }
}
