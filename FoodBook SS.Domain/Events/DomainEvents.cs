namespace FoodBook_SS.Domain.Events
{
    public sealed record ReservationCreated(
        int ReservaId, int ClienteId, int RestauranteId,
        DateOnly FechaReserva, TimeOnly HoraReserva, int NumeroPersonas,
        string CodigoConfirmacion, DateTime OcurridoEn);

    public sealed record ReservationCancelled(
        int ReservaId, int ActorId, string? Motivo, DateTime OcurridoEn);

    public sealed record OrderPlaced(
        int OrdenId, int ReservaId, int ClienteId, decimal Total, DateTime OcurridoEn);

    public sealed record PaymentProcessed(
        int PagoId, int OrdenId, int ClienteId, bool Aprobado,
        decimal Monto, string? CodigoAutorizacion, DateTime OcurridoEn);
}
