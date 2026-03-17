using FoodBook_SS.Domain.Base;
namespace FoodBook_SS.Domain.Entities.User
{
    public class Rol : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
    public class Usuario : BaseEntity
    {
        public int RolId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public bool Activo { get; set; } = true;
        public bool EmailConfirmado { get; set; } = false;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExp { get; set; }
        public Rol? Rol { get; set; }
    }
}
