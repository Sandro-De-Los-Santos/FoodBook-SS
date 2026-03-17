using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodBook_SS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRestaurantService _restaurantService;

        public HomeController(IRestaurantService restaurantService) =>
            _restaurantService = restaurantService;

        public async Task<IActionResult> Index(
            [FromQuery] string? ciudad, [FromQuery] string? tipo, [FromQuery] string? q)
        {
            var result = await _restaurantService.BuscarAsync(ciudad, tipo, q);
            return View(result.Data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
}

