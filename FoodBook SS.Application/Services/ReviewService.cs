using FoodBook_SS.Application.Base;
using FoodBook_SS.Application.Dtos.Review;
using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Review;
using FoodBook_SS.Domain.Repository;

namespace FoodBook_SS.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;
        private readonly IOrderRepository _orders;
        private readonly IAuditService _audit;
        private readonly IRestaurantRepository _restaurantRepo;

        public ReviewService(IReviewRepository repo, IOrderRepository orders, IAuditService audit, IRestaurantRepository restaurantRepo)
        { 
            _repo = repo; 
            _orders = orders; 
            _audit = audit;
            _restaurantRepo = restaurantRepo;
        }

        public async Task<OperationResult> GetAllAsync() => MapListToDto(await _repo.GetAllAsync(r => true));
        public async Task<OperationResult> GetByRestauranteAsync(int id) => MapListToDto(await _repo.GetByRestauranteAsync(id));
        public async Task<OperationResult> GetByClienteAsync(int clienteId) => MapListToDto(await _repo.GetByClienteIdAsync(clienteId));

        public async Task<OperationResult> GetByIdAsync(int id)
        {
            var r = await _repo.GetEntityByIdAsync(id);
            return r is null ? OperationResult.Fail("Reseña no encontrada.") : OperationResult.Ok(MapToDto(r));
        }

        public Task<OperationResult> SaveAsync(SaveReviewDto dto) => CreateAsync(dto, 0);

        
        public async Task<OperationResult> CreateAsync(SaveReviewDto dto, int clienteId)
        {
            var ver = await _orders.GetOrdenEntregadaParaResenaAsync(clienteId, dto.OrdenId);
            if (!ver.Success || ver.Data is null)
                return OperationResult.Fail("Solo puedes reseñar experiencias con una orden completada.");
            if (await _repo.ExistsAsync(r => r.OrdenId == dto.OrdenId))
                return OperationResult.Fail("Ya existe una reseña para esta orden.");
            if (dto.Calificacion < 1 || dto.Calificacion > 5)
                return OperationResult.Fail("La calificación debe ser entre 1 y 5.");
            var resena = new Resena
            {
                ClienteId = clienteId,
                RestauranteId = dto.RestauranteId,
                OrdenId = dto.OrdenId,
                Calificacion = dto.Calificacion,
                Comentario = dto.Comentario
            };
            var r = await _repo.SaveEntityAsync(resena);
            if (r.Success)
            {
                await _restaurantRepo.ActualizarCalificacionAsync(resena.RestauranteId);
            }
            return r;
        }

        public Task<OperationResult> UpdateAsync(int id, SaveReviewDto dto) =>
            Task.FromResult(OperationResult.Fail("Las reseñas publicadas no se pueden editar."));

        public async Task<OperationResult> ModerarAsync(int id, bool visible, int moderadorId)
        {
            var r = await _repo.ModerarAsync(id, visible, moderadorId);
            if (r.Success) 
            {
                await _audit.RegistrarAsync(moderadorId, "MODERAR_RESENA", "Resena", id.ToString(), datosNuevos: new { visible });
                
                var resenaRes = await _repo.GetEntityByIdAsync(id);
                if (resenaRes != null)
                {
                    await _restaurantRepo.ActualizarCalificacionAsync(resenaRes.RestauranteId);
                }
            }
            return r;
        }

        public async Task<OperationResult> ResponderAsync(int resenaId, string respuesta, int actorId)
        {
            if (string.IsNullOrWhiteSpace(respuesta))
                return OperationResult.Fail("La respuesta no puede estar vacía.");

            var result = await _repo.ResponderAsync(resenaId, respuesta);
            if (result.Success)
            {
                await _audit.RegistrarAsync(actorId, "RESPONDER_RESENA", "Resena", resenaId.ToString());
            }
            return result;
        }
        private static ReviewDto MapToDto(Resena r) => new()
        {
            Id = r.Id,
            RestauranteId = r.RestauranteId,
            NombreCliente = r.Cliente?.Nombre ?? "Cliente Anónimo",
            NombreRestaurante = r.Restaurante?.Nombre ?? string.Empty,
            Calificacion = r.Calificacion,
            Comentario = r.Comentario,
            Respuesta = r.Respuesta,
            Visible = r.Visible,
            CreadoEn = r.CreadoEn
        };

        private static OperationResult MapListToDto(OperationResult result)
        {
            if (result.Success && result.Data is IEnumerable<Resena> lista)
                return OperationResult.Ok(lista.Select(MapToDto).ToList());
            return result;
        }
    }
}
