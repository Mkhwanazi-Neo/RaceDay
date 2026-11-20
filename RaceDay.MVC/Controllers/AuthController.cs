using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;


namespace RaceDay.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public AuthController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _apiService.LoginAsync(model);
            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserName", user.Name);

            return user.Role == "Organiser"
                ? RedirectToAction("Dashboard", "Organiser")
                : RedirectToAction("Index", "Participant");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, error) = await _apiService.RegisterAsync(model);
            if (!success)
            {
                ViewBag.Error = "Registration failed. Please check your details and try again.";
                return View(model);
            }

            TempData["Success"] = "Account created! Please log in.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await _apiService.LogoutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}