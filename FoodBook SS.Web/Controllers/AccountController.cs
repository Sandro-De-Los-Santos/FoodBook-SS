using FoodBook_SS.Application.Dtos.User;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodBook_SS.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService) => _userService = userService;

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl)
        {
            var result = await _userService.LoginAsync(dto);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(dto);
            }


            var auth = (AuthResponseDto)result.Data!;
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, auth.Usuario.Id.ToString()),
                new(ClaimTypes.Email,          auth.Usuario.Email),
                new(ClaimTypes.Name,           auth.Usuario.NombreCompleto),
                new(ClaimTypes.Role,           auth.Usuario.Rol)
            };
            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("Cookies", principal);

            return Redirect(returnUrl ?? "/Home/Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(SaveUserDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var result = await _userService.SaveAsync(dto);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(dto);
            }
            TempData["Mensaje"] = "Registro exitoso. Inicia sesión.";
            return RedirectToAction("Login");
        }
    }
}