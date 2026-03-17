using FoodBook_SS.Application.Dtos.Order;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodBook_SS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService) => _orderService = orderService;

        [HttpGet("cliente")]
        public async Task<IActionResult> GetMisOrdenes() =>
            Ok(await _orderService.GetByClienteAsync(ObtenerUsuarioId()));

        [HttpGet("restaurante/{restauranteId:int}")]
        [Authorize(Roles = "Propietario,Administrador")]
        public async Task<IActionResult> GetByRestaurante(int restauranteId, [FromQuery] string? estado) =>
            Ok(await _orderService.GetByRestauranteAsync(restauranteId, estado));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _orderService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create([FromBody] SaveOrderDto dto)
        {
            var result = await _orderService.CreateAsync(dto, ObtenerUsuarioId());
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result)
                                  : BadRequest(result);
        }

        [HttpPost("{id:int}/items")]
        public async Task<IActionResult> AddItem(int id, [FromBody] SaveOrderItemDto dto)
        {
            var result = await _orderService.AddItemAsync(id, dto, ObtenerUsuarioId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("items/{itemId:int}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var result = await _orderService.RemoveItemAsync(itemId, ObtenerUsuarioId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id:int}/estado")]
        [Authorize(Roles = "Propietario,Administrador")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
        {
            var result = await _orderService.CambiarEstadoAsync(id, nuevoEstado, ObtenerUsuarioId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("restaurante/{restauranteId:int}/ventas")]
        [Authorize(Roles = "Propietario,Administrador")]
        public async Task<IActionResult> GetVentas(int restauranteId,
            [FromQuery] DateOnly desde, [FromQuery] DateOnly hasta) =>
            Ok(await _orderService.GetVentasAsync(restauranteId, desde, hasta));

        [HttpGet("restaurante/{restauranteId:int}/top-productos")]
        [Authorize(Roles = "Propietario,Administrador")]
        public async Task<IActionResult> GetTopProductos(int restauranteId,
            [FromQuery] DateOnly desde, [FromQuery] DateOnly hasta) =>
            Ok(await _orderService.GetProductosMasOrdenadosAsync(restauranteId, desde, hasta));

        private int ObtenerUsuarioId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
}
