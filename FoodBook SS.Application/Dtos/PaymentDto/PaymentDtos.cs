using FoodBook_SS.Application.Base;
namespace FoodBook_SS.Application.Dtos.Payment
{
    public class PaymentDto : DtoBase
    {
        public int OrdenId { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? CodigoAutorizacion { get; set; }
        public DateTime? FechaPago { get; set; }
    }
    public class SavePaymentDto
    {
        public int OrdenId { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
    }
}