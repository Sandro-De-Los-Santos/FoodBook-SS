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

        public ReviewService(IReviewRepository repo, IOrderRepository orders, IAuditService audit)
        { _repo = repo; _orders = orders; _audit = audit; }

        public Task<OperationResult> GetAllAsync() => _repo.GetAllAsync(r => r.Visible);
        public Task<OperationResult> GetByRestauranteAsync(int id) => _repo.GetByRestauranteAsync(id);
        public Task<OperationResult> GetByClienteAsync(int clienteId) => _repo.GetByClienteIdAsync(clienteId);

        public async Task<OperationResult> GetByIdAsync(int id)
        {
            var r = await _repo.GetEntityByIdAsync(id);
            return r is null ? OperationResult.Fail("Reseña no encontrada.") : OperationResult.Ok(r);
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
            return await _repo.SaveEntityAsync(resena);
        }

        public Task<OperationResult> UpdateAsync(int id, SaveReviewDto dto) =>
            Task.FromResult(OperationResult.Fail("Las reseñas publicadas no se pueden editar."));

        public async Task<OperationResult> ModerarAsync(int id, bool visible, int moderadorId)
        {
            var r = await _repo.ModerarAsync(id, visible, moderadorId);
            if (r.Success) await _audit.RegistrarAsync(moderadorId, "MODERAR_RESENA", "Resena", id.ToString(), datosNuevos: new { visible });
            return r;
        }
    }
}
