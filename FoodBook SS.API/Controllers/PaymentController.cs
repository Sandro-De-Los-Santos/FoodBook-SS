using FoodBook_SS.Application.Dtos.Payment;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodBook_SS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService) => _paymentService = paymentService;

        [HttpGet("orden/{ordenId:int}")]
        public async Task<IActionResult> GetByOrden(int ordenId) =>
            Ok(await _paymentService.GetByOrdenAsync(ordenId));

        [HttpGet("cliente")]
        public async Task<IActionResult> GetMisPagos() =>
            Ok(await _paymentService.GetByClienteAsync(ObtenerUsuarioId()));

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ProcesarPago([FromBody] SavePaymentDto dto)
        {
            var result = await _paymentService.ProcessAsync(dto, ObtenerUsuarioId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("restaurante/{restauranteId:int}/resumen")]
        [Authorize(Roles = "Propietario,Administrador")]
        public async Task<IActionResult> GetResumen(int restauranteId,
            [FromQuery] DateOnly desde, [FromQuery] DateOnly hasta) =>
            Ok(await _paymentService.GetResumenAsync(restauranteId, desde, hasta));

        private int ObtenerUsuarioId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
}
