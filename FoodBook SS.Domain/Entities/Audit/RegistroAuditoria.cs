namespace FoodBook_SS.Domain.Entities.Audit
{
    
    public class RegistroAuditoria
    {
        public long AuditoriaId { get; set; }
        public int? ActorId { get; set; }
        public string Accion { get; set; } = string.Empty;
        public string Entidad { get; set; } = string.Empty;
        public string? EntidadId { get; set; }
        public string? DatosAnteriores { get; set; }
        public string? DatosNuevos { get; set; }
        public string? IpOrigen { get; set; }
        public DateTime FechaHora { get; set; } = DateTime.UtcNow;
        public string Resultado { get; set; } = "Exito";
        public string? Detalle { get; set; }
    }
}

