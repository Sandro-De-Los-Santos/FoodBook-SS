using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Payment;
using FoodBook_SS.Domain.Entities.Reservation;
using FoodBook_SS.Domain.Repository;
using FoodBook_SS.Persistence.Base;
using FoodBook_SS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodBook_SS.Persistence.Repositories.Payment
{
    public class PaymentRepository : BaseRepositorycs<Pago>, IPaymentRepository
    {
        private readonly FoodBookDbContext _context;

        public PaymentRepository(FoodBookDbContext context) : base(context) => _context = context;

        public async Task<OperationResult> GetAllByOrdenIdAsync(int ordenId)
        {
            var lista = await _context.Pagos.Where(p => p.OrdenId == ordenId).ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public async Task<OperationResult> GetByClienteIdAsync(int clienteId)
        {
            var lista = await _context.Pagos
                .Where(p => p.ClienteId == clienteId)
                .OrderByDescending(p => p.CreadoEn)
                .ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public async Task<OperationResult> RegistrarPagoAsync(Pago pago)
        {
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
            return OperationResult.Ok(data: pago.Id);
        }

        public async Task<OperationResult> GetResumenTransaccionesAsync(int restauranteId, DateOnly desde, DateOnly hasta)
        {
            var resumen = await _context.Pagos
                .Where(p => p.Orden!.RestauranteId == restauranteId &&
                            p.Estado == EstadoPago.Aprobado &&
                            DateOnly.FromDateTime(p.CreadoEn) >= desde &&
                            DateOnly.FromDateTime(p.CreadoEn) <= hasta)
                .GroupBy(p => p.MetodoPago)
                .Select(g => new { MetodoPago = g.Key, Total = g.Sum(p => p.Monto), Cantidad = g.Count() })
                .ToListAsync();
            return OperationResult.Ok(data: resumen);
        }

        public async Task<OperationResult> GetPagoAprobadoByOrdenIdAsync(int ordenId)
        {
            var pago = await _context.Pagos
                .FirstOrDefaultAsync(p => p.OrdenId == ordenId && p.Estado == EstadoPago.Aprobado);
            return OperationResult.Ok(data: pago);
        }

        public async Task<OperationResult> ActualizarEstadoPagoAsync(int pagoId, string nuevoEstado,
                                                                      string? codigoAuth, string? referencia)
        {
            var rows = await _context.Pagos.Where(p => p.Id == pagoId).ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Estado, nuevoEstado)
                .SetProperty(p => p.CodigoAutorizacion, codigoAuth)
                .SetProperty(p => p.ReferenciaExterna, referencia)
                .SetProperty(p => p.ActualizadoEn, DateTime.UtcNow));
            return rows > 0 ? OperationResult.Ok() : OperationResult.Fail("Pago no encontrado.");
        }
    }
}
