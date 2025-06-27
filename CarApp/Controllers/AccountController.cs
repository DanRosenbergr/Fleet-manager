using CarApp.Models;
using CarApp.Services;
using CarApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarApp.Controllers {
    [Authorize]
    public class AccountController : Controller {

        UserManager<AppUser> _userManager;
        SignInManager<AppUser> _signInManager;
        DemoDataService _demoDataService; //demo

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, DemoDataService demoDataService) {
            _userManager = userManager;
            _signInManager = signInManager;
            _demoDataService = demoDataService;//demo
        }

        [AllowAnonymous]
        public IActionResult Login() {
            LoginViewModel model = new LoginViewModel();           
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login, bool demoUser) {

            //demo data
            if (demoUser) {
                var demoUserName = "tryMe";
                var demoPassword = "tryMe1234";

                var user = await _userManager.FindByNameAsync(demoUserName);
                if (user != null) {
                    var result = await _signInManager.PasswordSignInAsync(demoUserName, demoPassword, isPersistent: false, lockoutOnFailure: false);
                    if (result.Succeeded) {
                        return RedirectToAction("Index", "Cars");
                    }
                }

                TempData["Error"] = "Demo login failed.";
                return RedirectToAction("Login");
            }


            if (ModelState.IsValid) {
                AppUser userToLogin = await _userManager.FindByNameAsync(login.UserName);                
                if (userToLogin == null) {
                    ModelState.AddModelError("", "User not found");
                } else {
                    var signInResult = await _signInManager.PasswordSignInAsync(userToLogin,
                        login.Password, login.Remember, false);

                    if (signInResult.Succeeded) {
                        return RedirectToAction("Index", "Cars"); 
                    } else {
                        ModelState.AddModelError("", "Wrong password");
                    }
                }
            }
            ModelState.AddModelError("", "User not found or wrong password");
            login.Password = String.Empty;
            return View(login);
        }


        public async Task<IActionResult> Logout() {

            var user = await _userManager.GetUserAsync(User);
            if (user != null && user.UserName.StartsWith("tryMe-")) {
                // Smazat data
                await _demoDataService.CleanupDemoDataAsync(user.Id);
                await _userManager.DeleteAsync(user);
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //Edit Profile methods
        [HttpGet]
        public async Task<IActionResult> EditProfile() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new EditProfileViewModel {
                UserName = user.UserName
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model) {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            
            //demo account edit protection
            if (user.UserName == "tryMe") {
                TempData["Message"] = "Demo account cannot be edited.";
                return RedirectToAction("Index", "Home");
            }
            // UserName change
            if (user.UserName != model.UserName) {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                if (!setUserNameResult.Succeeded) {
                    foreach (var error in setUserNameResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(model);
                }
                TempData["UserNameMessage"] = "Username was changed successfully.";
            }

           //passwordchange
            if (!string.IsNullOrEmpty(model.NewPassword)) {
                if (string.IsNullOrEmpty(model.CurrentPassword)) {
                    ModelState.AddModelError("CurrentPassword", "You must enter your current password.");
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
            //successfull change login again
            await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction(nameof(EditProfile));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (!ModelState.IsValid)
                return View(model);

            var newUser = new AppUser { UserName = model.UserName };
            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded) {
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors) {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        //Demo Account
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsDemo() {

            var demoSuffix = Guid.NewGuid().ToString().Substring(0, 8);
            var demoUsername = $"tryMe-{demoSuffix}";
            var demoPassword = "Demo123!";

            var demoUser = new AppUser {
                UserName = demoUsername,               
            };

            var result = await _userManager.CreateAsync(demoUser, demoPassword);
            if (!result.Succeeded) return RedirectToAction("Login");
                        
            await _userManager.AddToRoleAsync(demoUser, "User");
           
            await _demoDataService.SeedDemoDataAsync(demoUser.Id);

            await _signInManager.PasswordSignInAsync(demoUser.UserName, demoPassword, isPersistent: false, false);
            return RedirectToAction("Index", "Cars");
        }

    }
}
