using FoodBook_SS.Domain.Entities.User;
namespace FoodBook_SS.Domain.Repository
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
    public interface IJwtTokenService
    {
        (string Token, string RefreshToken, DateTime ExpiresAt) Generate(Usuario usuario);
        int? ValidateAndGetUserId(string token);
    }
    public interface IPaymentGateway
    {
        Task<GatewayResult> ProcesarPagoAsync(string referencia, decimal monto, string metodo);
    }
    public record GatewayResult(bool Aprobado, string? CodigoAutorizacion, string Mensaje);
}
