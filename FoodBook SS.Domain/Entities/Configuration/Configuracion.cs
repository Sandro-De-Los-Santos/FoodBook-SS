using FoodBook_SS.Domain.Base;

namespace FoodBook_SS.Domain.Entities.Configuration
{
    public class Restaurante : BaseEntity
    {
        public int PropietarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? TipoCocina { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? RangoPrecio { get; set; }
        public decimal CalificacionProm { get; set; } = 0;
        public int TotalResenas { get; set; } = 0;
        public bool Activo { get; set; } = true;
        public ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();
        public ICollection<CategoriaMenu> Categorias { get; set; } = new List<CategoriaMenu>();
    }

    public class Mesa : BaseEntity
    {
        public int RestauranteId { get; set; }
        public string NumeroMesa { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public string? Ubicacion { get; set; }
        public bool Activa { get; set; } = true;
        public Restaurante? Restaurante { get; set; }
    }

    public class HorarioRestaurante
    {
        public int HorarioId { get; set; }
        public int RestauranteId { get; set; }
        public int DiaSemana { get; set; }
        public TimeOnly AbreA { get; set; }
        public TimeOnly CierraA { get; set; }
        public bool Activo { get; set; } = true;
        public Restaurante? Restaurante { get; set; }
    }

    public class CategoriaMenu : BaseEntity
    {
        public int RestauranteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int Orden { get; set; } = 0;
        public bool Activa { get; set; } = true;
        public ICollection<ProductoMenu> Productos { get; set; } = new List<ProductoMenu>();
    }

    public class ProductoMenu : BaseEntity
    {
        public int CategoriaId { get; set; }
        public int RestauranteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public bool Disponible { get; set; } = true;
        public string? Imagen { get; set; }
        public CategoriaMenu? Categoria { get; set; }
    }
}
