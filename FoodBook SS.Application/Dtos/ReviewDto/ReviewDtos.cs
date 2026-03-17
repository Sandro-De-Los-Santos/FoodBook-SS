using FoodBook_SS.Application.Base;
namespace FoodBook_SS.Application.Dtos.Review
{
    public class ReviewDto : DtoBase
    {
        public string NombreCliente { get; set; } = string.Empty;
        public string NombreRestaurante { get; set; } = string.Empty;
        public byte Calificacion { get; set; }
        public string? Comentario { get; set; }
        public DateTime CreadoEn { get; set; }
    }
    public class SaveReviewDto
    {
        public int RestauranteId { get; set; }
        public int OrdenId { get; set; }
        public byte Calificacion { get; set; }
        public string? Comentario { get; set; }
    }
}
