using FoodBook_SS.Application.Base;
namespace FoodBook_SS.Application.Dtos.Reservation
{
    public class ReservationDto : DtoBase
    {
        public string NombreCliente { get; set; } = string.Empty;
        public string NombreRestaurante { get; set; } = string.Empty;
        public DateOnly FechaReserva { get; set; }
        public TimeOnly HoraReserva { get; set; }
        public int NumeroPersonas { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? CodigoConfirmacion { get; set; }
        public string? NumeroMesa { get; set; }
    }
    public class SaveReservationDto
    {
        public int RestauranteId { get; set; }
        public DateOnly FechaReserva { get; set; }
        public TimeOnly HoraReserva { get; set; }
        public int NumeroPersonas { get; set; }
        public string? NotasEspeciales { get; set; }
    }
    public class UpdateReservationDto
    {
        public DateOnly? FechaReserva { get; set; }
        public TimeOnly? HoraReserva { get; set; }
        public int? NumeroPersonas { get; set; }
        public string? NotasEspeciales { get; set; }
    }
}
