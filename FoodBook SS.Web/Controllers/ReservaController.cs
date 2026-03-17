using FoodBook_SS.Application.Dtos.Reservation;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodBook_SS.Web.Controllers
{
    [Authorize]
    public class ReservaController : Controller
    {
        private readonly IReservationService _reservationService;

        public ReservaController(IReservationService reservationService) =>
            _reservationService = reservationService;

        public async Task<IActionResult> Index()
        {
            var result = await _reservationService.GetAllByClienteAsync(ObtenerUsuarioId());
            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Crear(int restauranteId) => View(new SaveReservationDto { RestauranteId = restauranteId });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(SaveReservationDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var result = await _reservationService.CreateAsync(dto, ObtenerUsuarioId());
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(dto);
            }
            TempData["Mensaje"] = "Reserva creada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id, string? motivo)
        {
            await _reservationService.CancelarAsync(id, ObtenerUsuarioId(), motivo);
            return RedirectToAction(nameof(Index));
        }

        private int ObtenerUsuarioId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
}

