using CarApp.Models;
using CarApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarApp.Controllers {
    public class AccountController : Controller {

        UserManager<AppUser> _userManager;
        SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl) {
            LoginViewModel model = new LoginViewModel();
            model.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login) {
            if (ModelState.IsValid) {
                AppUser userToLogin = await _userManager.FindByNameAsync(login.UserName);
                if (userToLogin != null) {
                    var signInResult = await _signInManager.PasswordSignInAsync(userToLogin,
                        login.Password, login.Remember, false);//zmena, pri pridani checkboxu(login.Remember)
                    if (signInResult.Succeeded) {
                        return Redirect(login.ReturnUrl ?? "/cars/index");
                    }
                }
            }
            ModelState.AddModelError("", "User not found or wrong password");
            login.Password = String.Empty;
            return View(login);
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

            var newUser = new AppUser {
                UserName = model.UserName,                
            };

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


        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
