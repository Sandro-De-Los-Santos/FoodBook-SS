using FoodBook_SS.Application.Base;
using FoodBook_SS.Application.Dtos.Restaurant;
using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Configuration;
using FoodBook_SS.Domain.Repository;

namespace FoodBook_SS.Application.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _repo;

        public RestaurantService(IRestaurantRepository repo) => _repo = repo;

        public async Task<OperationResult> GetAllAsync() => MapListToDto(await _repo.GetAllAsync(r => true));
        public async Task<OperationResult> GetByPropietarioAsync(int id) => MapListToDto(await _repo.GetByPropietarioAsync(id));
        public async Task<OperationResult> SearchAsync(string? nombre, string? ciudad, string? tipoCocina) =>
            MapListToDto(await _repo.SearchAsync(nombre, ciudad, tipoCocina));

        public async Task<OperationResult> BuscarAsync(string? ciudad, string? tipoCocina, string? termino) =>
            MapListToDto(await _repo.SearchAsync(termino, ciudad, tipoCocina));

        public async Task<OperationResult> GetByIdAsync(int id)
        {
            var r = await _repo.GetEntityByIdAsync(id);
            return r is null ? OperationResult.Fail("Restaurante no encontrado.") : OperationResult.Ok(MapToDto(r));
        }

        public async Task<OperationResult> SaveAsync(SaveRestaurantDto dto)
        {
            var r = new Restaurante
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                TipoCocina = dto.TipoCocina,
                Direccion = dto.Direccion,
                Ciudad = dto.Ciudad,
                Telefono = dto.Telefono,
                RangoPrecio = dto.RangoPrecio
            };
            return await _repo.SaveEntityAsync(r);
        }

        public async Task<OperationResult> CreateAsync(SaveRestaurantDto dto, int propietarioId)
        {
            var r = new Restaurante
            {
                PropietarioId = propietarioId,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                TipoCocina = dto.TipoCocina,
                Direccion = dto.Direccion,
                Ciudad = dto.Ciudad,
                Telefono = dto.Telefono,
                RangoPrecio = dto.RangoPrecio
            };
            return await _repo.SaveEntityAsync(r);
        }

        public async Task<OperationResult> UpdateAsync(int id, UpdateRestaurantDto dto)
        {
            var r = await _repo.GetEntityByIdAsync(id);
            if (r is null) return OperationResult.Fail("Restaurante no encontrado.");
            if (dto.Descripcion is not null) r.Descripcion = dto.Descripcion;
            if (dto.Telefono is not null) r.Telefono = dto.Telefono;
            if (dto.RangoPrecio is not null) r.RangoPrecio = dto.RangoPrecio;
            return await _repo.UpdateEntityAsync(r);
        }

        public async Task<OperationResult> AgregarMesaAsync(SaveMesaDto dto)
        {
            var mesa = new Mesa
            {
                RestauranteId = dto.RestauranteId,
                NumeroMesa    = dto.NumeroMesa,
                Capacidad     = dto.Capacidad,
                Ubicacion     = dto.Ubicacion,
                Activa        = true
            };
            return await _repo.SaveMesaAsync(mesa);
        }

        public async Task<OperationResult> ToggleEstadoAsync(int restauranteId, bool activo, int actorId)
        {
            var r = await _repo.GetEntityByIdAsync(restauranteId);
            if (r is null) return OperationResult.Fail("Restaurante no encontrado.");
            
            r.Activo = activo;
            r.ActualizadoEn = DateTime.UtcNow;
            r.ModificadoPor = actorId;
            
            return await _repo.UpdateEntityAsync(r);
        }

        private static RestaurantDto MapToDto(Restaurante r) => new()
        {
            Id = r.Id,
            PropietarioId = r.PropietarioId,
            Nombre = r.Nombre,
            TipoCocina = r.TipoCocina,
            Direccion = r.Direccion,
            Ciudad = r.Ciudad,
            RangoPrecio = r.RangoPrecio,
            CalificacionProm = r.CalificacionProm,
            TotalResenas = r.TotalResenas,
            Activo = r.Activo,
            Mesas = r.Mesas?.Select(m => new MesaDto
            {
                Id = m.Id,
                NumeroMesa = m.NumeroMesa,
                Capacidad = m.Capacidad,
                Ubicacion = m.Ubicacion,
                Disponible = true
            }).ToList() ?? new()
        };

        private static OperationResult MapListToDto(OperationResult result)
        {
            if (result.Success && result.Data is IEnumerable<Restaurante> lista)
                return OperationResult.Ok(lista.Select(MapToDto).ToList());
            return result;
        }
    }
}