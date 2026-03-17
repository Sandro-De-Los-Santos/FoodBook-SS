using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Order;
using FoodBook_SS.Domain.Entities.Reservation;
namespace FoodBook_SS.Domain.Entities.Payment
{
    public class Pago : BaseEntity
    {
        public int OrdenId { get; set; }
        public int ClienteId { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string Estado { get; set; } = EstadoPago.Pendiente;
        public string? CodigoAutorizacion { get; set; }
        public string? ReferenciaExterna { get; set; }
        public string? MensajeRespuesta { get; set; }
        public DateTime? FechaPago { get; set; }
        public Orden? Orden { get; set; }
    }
}

