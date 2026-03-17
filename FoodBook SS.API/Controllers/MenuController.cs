using FoodBook_SS.Application.Dtos.Menu;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodBook_SS.API.Controllers
{
    [Authorize]
    public class MenuController : BaseApiController
    {
        private readonly IMenuService _service;
        public MenuController(IMenuService service) => _service = service;

        [AllowAnonymous]
        [HttpGet("{restauranteId:int}/categorias")]
        public async Task<IActionResult> GetCategorias(int restauranteId)
            => Respond(await _service.GetCategoriasByRestauranteAsync(restauranteId));

        [AllowAnonymous]
        [HttpGet("{restauranteId:int}/productos")]
        public async Task<IActionResult> GetProductos(int restauranteId)
            => Respond(await _service.GetProductosByRestauranteAsync(restauranteId));

        [Authorize(Roles = "Propietario,Administrador")]
        [HttpPost("categorias")]
        public async Task<IActionResult> SaveCategoria([FromBody] SaveCategoryDto dto)
            => Respond(await _service.SaveCategoriaAsync(dto));

        [Authorize(Roles = "Propietario,Administrador")]
        [HttpPost("productos")]
        public async Task<IActionResult> SaveProducto([FromBody] SaveProductDto dto)
            => Respond(await _service.SaveProductoAsync(dto));

        [Authorize(Roles = "Propietario,Administrador")]
        [HttpPut("productos/{id:int}")]
        public async Task<IActionResult> UpdateProducto(int id, [FromBody] UpdateProductDto dto)
            => Respond(await _service.UpdateProductoAsync(id, dto));

        [Authorize(Roles = "Propietario,Administrador")]
        [HttpPatch("productos/{id:int}/disponibilidad")]
        public async Task<IActionResult> ToggleDisponibilidad(int id)
            => Respond(await _service.ToggleDisponibilidadAsync(id, ObtenerUsuarioId()));
    }
}
