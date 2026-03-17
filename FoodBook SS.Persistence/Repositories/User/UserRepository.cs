using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.User;
using FoodBook_SS.Domain.Repository;
using FoodBook_SS.Persistence.Base;
using FoodBook_SS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodBook_SS.Persistence.Repositories.User
{
    public class UserRepository : BaseRepositorycs<Usuario>, IUserRepository
    {
        private readonly FoodBookDbContext _context;

        public UserRepository(FoodBookDbContext context) : base(context) => _context = context;

        public Task<Usuario?> GetByEmailAsync(string email) =>
            _context.Usuarios.Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == email);

        public Task<bool> EmailExisteAsync(string email) =>
            _context.Usuarios.AnyAsync(u => u.Email == email);

        public async Task<OperationResult> GetByRolAsync(int rolId)
        {
            var lista = await _context.Usuarios
                .Where(u => u.RolId == rolId).ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public async Task<OperationResult> ActualizarRefreshTokenAsync(int usuarioId, string? token, DateTime? expira)
        {
            var rows = await _context.Usuarios
                .Where(u => u.Id == usuarioId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.RefreshToken, token)
                    .SetProperty(u => u.RefreshTokenExp, expira)
                    .SetProperty(u => u.ActualizadoEn, DateTime.UtcNow));
            return rows > 0 ? OperationResult.Ok() : OperationResult.Fail("Usuario no encontrado.");
        }

        public async Task<OperationResult> CambiarPasswordAsync(int usuarioId, string nuevoHash)
        {
            var rows = await _context.Usuarios
                .Where(u => u.Id == usuarioId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.PasswordHash, nuevoHash)
                    .SetProperty(u => u.ActualizadoEn, DateTime.UtcNow));
            return rows > 0 ? OperationResult.Ok() : OperationResult.Fail("Usuario no encontrado.");
        }

        public async Task<OperationResult> ActivarDesactivarAsync(int usuarioId, bool activo, int actorId)
        {
            var rows = await _context.Usuarios
                .Where(u => u.Id == usuarioId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.Activo, activo)
                    .SetProperty(u => u.ModificadoPor, actorId)
                    .SetProperty(u => u.ActualizadoEn, DateTime.UtcNow));
            return rows > 0 ? OperationResult.Ok() : OperationResult.Fail("Usuario no encontrado.");
        }

        public async Task<List<Rol>> GetRolesAsync() =>
            await _context.Roles.ToListAsync();
    }
}
