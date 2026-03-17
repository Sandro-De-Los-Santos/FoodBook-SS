using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Configuration;
using FoodBook_SS.Domain.Entities.Reservation;

namespace FoodBook_SS.Domain.Entities.Order
{
    public class Orden : BaseEntity
    {
        public int ReservaId { get; set; }
        public int ClienteId { get; set; }
        public int RestauranteId { get; set; }
        public string Estado { get; set; } = EstadoOrden.Pendiente;
        public decimal Subtotal { get; set; } = 0;
        public decimal Impuesto { get; set; } = 0;
        public decimal Total { get; set; } = 0;
        public string? Notas { get; set; }
        public ICollection<ItemOrden> Items { get; set; } = new List<ItemOrden>();
        
        public Restaurante? Restaurante { get; set; }
    }

    public class ItemOrden : BaseEntity
    {
        public int OrdenId { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => PrecioUnitario * Cantidad;
        public string? Notas { get; set; }
        public Orden? Orden { get; set; }
    }
}
