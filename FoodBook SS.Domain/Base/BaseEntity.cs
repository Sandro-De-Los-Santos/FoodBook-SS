namespace FoodBook_SS.Domain.Base
{
    
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? ActualizadoEn { get; set; }
        public int? CreadoPor { get; set; }
        public int? ModificadoPor { get; set; }
        public bool Eliminado { get; set; } = false;
    }
}

