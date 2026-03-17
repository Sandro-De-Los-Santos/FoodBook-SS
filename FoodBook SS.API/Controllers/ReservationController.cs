using FoodBook_SS.Application.Dtos.Reservation;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodBook_SS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService) =>
            _reservationService = reservationService;

        [HttpGet("cliente")]
        public async Task<IActionResult> GetMisReservas()
        {
            var result = await _reservationService.GetAllByClienteAsync(ObtenerUsuarioId());
            return Ok(result);
        }

        [HttpGet("restaurante/{restauranteId:int}")]
        [Authorize(Roles = "Propietario,Administrador")]
        public async Task<IActionResult> GetByRestaurante(int restauranteId, [FromQuery] string? estado)
        {
            var result = await _reservationService.GetAllByRestauranteAsync(restauranteId, estado);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _reservationService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("codigo/{codigo}")]
        public async Task<IActionResult> GetByCodigo(string codigo)
        {
            var result = await _reservationService.GetByCodigoAsync(codigo);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("disponibilidad")]
        public async Task<IActionResult> GetMesasDisponibles(
            [FromQuery] int restauranteId, [FromQuery] DateOnly fecha,
            [FromQuery] TimeOnly hora, [FromQuery] int personas)
        {
            var result = await _reservationService.GetMesasDisponiblesAsync(restauranteId, fecha, hora, personas);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create([FromBody] SaveReservationDto dto)
        {
            var result = await _reservationService.CreateAsync(dto, ObtenerUsuarioId());
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result)
                                  : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReservationDto dto)
        {
            var result = await _reservationService.UpdateAsync(id, dto, ObtenerUsuarioId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id:int}/confirmar")]
        [Authorize(Roles = "Propietario,Administrador")]
        public async Task<IActionResult> Confirmar(int id)
        {
            var result = await _reservationService.ConfirmarAsync(id, ObtenerUsuarioId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id:int}/cancelar")]
        public async Task<IActionResult> Cancelar(int id, [FromBody] string? motivo)
        {
            var result = await _reservationService.CancelarAsync(id, ObtenerUsuarioId(), motivo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id:int}/completar")]
        [Authorize(Roles = "Propietario,Administrador")]
        public async Task<IActionResult> Completar(int id)
        {
            var result = await _reservationService.CompletarAsync(id, ObtenerUsuarioId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id:int}/noshow")]
        [Authorize(Roles = "Propietario,Administrador")]
        public async Task<IActionResult> NoShow(int id)
        {
            var result = await _reservationService.MarcarNoShowAsync(id, ObtenerUsuarioId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private int ObtenerUsuarioId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
}
