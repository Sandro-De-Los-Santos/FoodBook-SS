using FoodBook_SS.Application.Base;
namespace FoodBook_SS.Application.Dtos.Restaurant
{
    public class RestaurantDto : DtoBase
    {
        public string Nombre { get; set; } = string.Empty;
        public string? TipoCocina { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string? RangoPrecio { get; set; }
        public decimal CalificacionProm { get; set; }
        public int TotalResenas { get; set; }
    }
    public class SaveRestaurantDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? TipoCocina { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? RangoPrecio { get; set; }
    }
    public class UpdateRestaurantDto
    {
        public string? Descripcion { get; set; }
        public string? Telefono { get; set; }
        public string? RangoPrecio { get; set; }
    }
}

