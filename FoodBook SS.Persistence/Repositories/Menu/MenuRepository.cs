using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Configuration;
using FoodBook_SS.Domain.Repository;
using FoodBook_SS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodBook_SS.Persistence.Repositories.Menu
{
    public class MenuRepository : IMenuRepository
    {
        private readonly FoodBookDbContext _context;

        public MenuRepository(FoodBookDbContext context) => _context = context;

        

        public async Task<OperationResult> GetCategoriasByRestauranteAsync(int restauranteId)
        {
            var lista = await _context.CategoriasMenu
                .Where(c => c.RestauranteId == restauranteId && c.Activa)
                .Include(c => c.Productos.Where(p => p.Disponible))
                .OrderBy(c => c.Orden)
                .ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public async Task<OperationResult> SaveCategoriaAsync(CategoriaMenu categoria)
        {
            _context.CategoriasMenu.Add(categoria);
            await _context.SaveChangesAsync();
            return OperationResult.Ok(data: categoria.Id);
        }

        public async Task<OperationResult> UpdateCategoriaAsync(CategoriaMenu categoria)
        {
            _context.CategoriasMenu.Update(categoria);
            await _context.SaveChangesAsync();
            return OperationResult.Ok();
        }

        

        public async Task<OperationResult> GetProductosByRestauranteAsync(int restauranteId, bool soloDisponibles = false)
        {
            var query = _context.ProductosMenu
                .Where(p => p.RestauranteId == restauranteId);
            if (soloDisponibles) query = query.Where(p => p.Disponible);
            var lista = await query.Include(p => p.Categoria).OrderBy(p => p.Nombre).ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public async Task<OperationResult> GetProductosByCategoriaAsync(int categoriaId)
        {
            var lista = await _context.ProductosMenu
                .Where(p => p.CategoriaId == categoriaId)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public async Task<OperationResult> SaveProductoAsync(ProductoMenu producto)
        {
            _context.ProductosMenu.Add(producto);
            await _context.SaveChangesAsync();
            return OperationResult.Ok(data: producto.Id);
        }

        public async Task<OperationResult> UpdateProductoAsync(ProductoMenu producto)
        {
            _context.ProductosMenu.Update(producto);
            await _context.SaveChangesAsync();
            return OperationResult.Ok();
        }

        public async Task<OperationResult> CambiarDisponibilidadAsync(int productoId, bool disponible, int actorId)
        {
            var rows = await _context.ProductosMenu
                .Where(p => p.Id == productoId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Disponible, disponible)
                    .SetProperty(p => p.ModificadoPor, actorId)
                    .SetProperty(p => p.ActualizadoEn, DateTime.UtcNow));
            return rows > 0 ? OperationResult.Ok() : OperationResult.Fail("Producto no encontrado.");
        }

        public Task<ProductoMenu?> GetProductoByIdAsync(int productoId) =>
            _context.ProductosMenu.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == productoId);
    }
}
