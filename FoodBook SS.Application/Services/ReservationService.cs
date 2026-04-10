using FoodBook_SS.Application.Base;
using FoodBook_SS.Application.Dtos.Reservation;
using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Reservation;
using FoodBook_SS.Domain.Repository;

namespace FoodBook_SS.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _repo;
        private readonly IAuditService _audit;
        private readonly INotificationService _notify;

        public ReservationService(IReservationRepository repo, IAuditService audit, INotificationService notify)
        { _repo = repo; _audit = audit; _notify = notify; }

        public async Task<OperationResult> GetAllAsync() => MapListToDto(await _repo.GetAllAsync(r => true));

        public async Task<OperationResult> GetByClienteAsync(int clienteId) => MapListToDto(await _repo.GetByClienteIdAsync(clienteId));

        
        public async Task<OperationResult> GetAllByClienteAsync(int clienteId) => MapListToDto(await _repo.GetByClienteIdAsync(clienteId));

        public async Task<OperationResult> GetByRestauranteAsync(int restauranteId, string? estado)
        {
            var result = estado is null
                ? await _repo.GetByRestauranteAndFechaAsync(restauranteId, DateOnly.FromDateTime(DateTime.Today))
                : await _repo.GetByRestauranteAndEstadoAsync(restauranteId, estado);
            return MapListToDto(result);
        }

        
        public async Task<OperationResult> GetAllByRestauranteAsync(int restauranteId, string? estado)
        {
            OperationResult result;
            if (estado is null)
            {
                // Retorna todas las reservas del restaurante sin filtro de fecha
                result = await _repo.GetAllByRestauranteAsync(restauranteId);
            }
            else
            {
                result = await _repo.GetByRestauranteAndEstadoAsync(restauranteId, estado);
            }
            return MapListToDto(result);
        }

        public async Task<OperationResult> GetByCodigoAsync(string codigo)
        {
            var result = await _repo.GetByCodigoConfirmacionAsync(codigo);
            if (result.Success && result.Data is Reserva r)
                return OperationResult.Ok(MapToDto(r));
            return result;
        }

        public Task<OperationResult> GetMesasDisponiblesAsync(int rid, DateOnly fecha, TimeOnly hora, int personas) =>
            _repo.GetMesasDisponiblesAsync(rid, fecha, hora, personas);

        public async Task<OperationResult> GetByIdAsync(int id)
        {
            var r = await _repo.GetEntityByIdAsync(id);
            return r is null ? OperationResult.Fail("Reserva no encontrada.") : OperationResult.Ok(MapToDto(r));
        }

        public Task<OperationResult> SaveAsync(SaveReservationDto dto) => CreateAsync(dto, 0);

        
        public async Task<OperationResult> CreateAsync(SaveReservationDto dto, int clienteId)
        {
            var disp = await _repo.GetMesasDisponiblesAsync(dto.RestauranteId, dto.FechaReserva, dto.HoraReserva, dto.NumeroPersonas);
            if (!disp.Success) return disp;

            var mesas = (IEnumerable<FoodBook_SS.Domain.Entities.Configuration.Mesa>)disp.Data;
            if (!mesas.Any(m => m.Id == dto.MesaId))
                return OperationResult.Fail("La mesa seleccionada no está disponible o no tiene capacidad suficiente.");

            var reserva = new Reserva
            {
                ClienteId = clienteId,
                RestauranteId = dto.RestauranteId,
                MesaId = dto.MesaId,
                FechaReserva = dto.FechaReserva,
                HoraReserva = dto.HoraReserva,
                NumeroPersonas = dto.NumeroPersonas,
                NotasEspeciales = dto.NotasEspeciales,
                Estado = EstadoReserva.Pendiente,
                CodigoConfirmacion = Guid.NewGuid().ToString("N")[..8].ToUpper()
            };
            var r = await _repo.SaveEntityAsync(reserva);
            if (!r.Success) return r;
            await _audit.RegistrarAsync(clienteId, "CREATE_RESERVA", "Reserva",
                reserva.Id.ToString(), datosNuevos: new { reserva.Estado, reserva.FechaReserva });
            await _notify.EnviarConfirmacionReservaAsync(clienteId, reserva.Id);
            return OperationResult.Ok(reserva.Id, "Reserva creada.");
        }

        public async Task<OperationResult> UpdateAsync(int id, UpdateReservationDto dto)
        {
            var r = await _repo.GetEntityByIdAsync(id);
            if (r is null) return OperationResult.Fail("Reserva no encontrada.");
            if (r.Estado == EstadoReserva.Cancelada || r.Estado == EstadoReserva.Completada)
                return OperationResult.Fail($"No se puede modificar una reserva en estado {r.Estado}.");
            if (dto.FechaReserva.HasValue) r.FechaReserva = dto.FechaReserva.Value;
            if (dto.HoraReserva.HasValue) r.HoraReserva = dto.HoraReserva.Value;
            if (dto.NumeroPersonas.HasValue) r.NumeroPersonas = dto.NumeroPersonas.Value;
            if (dto.NotasEspeciales is not null) r.NotasEspeciales = dto.NotasEspeciales;
            r.ActualizadoEn = DateTime.UtcNow;
            return await _repo.UpdateEntityAsync(r);
        }

        
        public async Task<OperationResult> UpdateAsync(int id, UpdateReservationDto dto, int actorId)
        {
            var result = await UpdateAsync(id, dto);
            if (result.Success)
                await _audit.RegistrarAsync(actorId, "UPDATE_RESERVA", "Reserva", id.ToString());
            return result;
        }

        public async Task<OperationResult> ConfirmarAsync(int id, int actorId)
        {
            var r = await _repo.ConfirmarReservaAsync(id, actorId);
            if (r.Success)
            {
                var reserva = await _repo.GetEntityByIdAsync(id);
                if (reserva is not null)
                    await _notify.EnviarConfirmacionReservaAsync(reserva.ClienteId, id);
            }
            return r;
        }

        public async Task<OperationResult> CancelarAsync(int id, int actorId, string? motivo)
        {
            var reserva = await _repo.GetEntityByIdAsync(id);
            if (reserva is null) return OperationResult.Fail("Reserva no encontrada.");
            var r = await _repo.CancelarReservaAsync(id, actorId, motivo);
            if (r.Success) await _notify.EnviarCancelacionReservaAsync(reserva.ClienteId, id, motivo);
            return r;
        }

        public Task<OperationResult> CompletarAsync(int id, int actorId) => _repo.CompletarReservaAsync(id, actorId);
        public Task<OperationResult> MarcarNoShowAsync(int id, int actorId) => _repo.MarcarNoShowAsync(id, actorId);

        private static ReservationDto MapToDto(Reserva r) => new()
        {
            Id = r.Id,
            NombreCliente = r.Cliente?.Nombre ?? "Cliente Anónimo",
            NombreRestaurante = r.Restaurante?.Nombre ?? string.Empty,
            FechaReserva = r.FechaReserva,
            HoraReserva = r.HoraReserva,
            NumeroPersonas = r.NumeroPersonas,
            Estado = r.Estado,
            CodigoConfirmacion = r.CodigoConfirmacion,
            NumeroMesa = r.Mesa?.NumeroMesa,
            RestauranteId = r.RestauranteId
        };

        private static OperationResult MapListToDto(OperationResult result)
        {
            if (result.Success && result.Data is IEnumerable<Reserva> lista)
                return OperationResult.Ok(lista.Select(MapToDto).ToList());
            return result;
        }
    }
}
