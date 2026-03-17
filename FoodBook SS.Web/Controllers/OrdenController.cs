using FoodBook_SS.Application.Dtos.Order;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodBook_SS.Web.Controllers
{
    [Authorize]
    public class OrdenController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMenuService _menuService;
        private readonly IPaymentService _paymentService;

        public OrdenController(IOrderService orderService, IMenuService menuService,
                                IPaymentService paymentService)
        {
            _orderService = orderService;
            _menuService = menuService;
            _paymentService = paymentService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _orderService.GetByClienteAsync(ObtenerUsuarioId());
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Crear(int restauranteId)
        {

            var menu = await _menuService.GetProductosByRestauranteAsync(restauranteId, true);
            ViewBag.RestauranteId = restauranteId;
            return View(menu.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(SaveOrderDto dto)
        {
            var result = await _orderService.CreateAsync(dto, ObtenerUsuarioId());
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Crear), new { restauranteId = dto.RestauranteId });
            }
            return RedirectToAction(nameof(Index));
        }

        private int ObtenerUsuarioId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
}
