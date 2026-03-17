using FoodBook_SS.Application.Dtos.User;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodBook_SS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) => _userService = userService;



        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] SaveUserDto dto)
        {
            var result = await _userService.SaveAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _userService.LoginAsync(dto);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _userService.RefreshTokenAsync(refreshToken);
            return result.Success ? Ok(result) : Unauthorized(result);
        }



        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var actorId = ObtenerUsuarioId();
            var result = await _userService.UpdateAsync(id, dto, actorId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id:int}/password")]
        [Authorize]
        public async Task<IActionResult> CambiarPassword(int id,
            [FromBody] CambiarPasswordRequest req)
        {
            var result = await _userService.ChangePasswordAsync(id, req.PasswordActual, req.PasswordNueva);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id:int}/estado")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ActivarDesactivar(int id, [FromQuery] bool activo)
        {
            var actorId = ObtenerUsuarioId();
            var result = await _userService.ActivarDesactivarAsync(id, activo, actorId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private int ObtenerUsuarioId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }

    public record CambiarPasswordRequest(string PasswordActual, string PasswordNueva);
}
