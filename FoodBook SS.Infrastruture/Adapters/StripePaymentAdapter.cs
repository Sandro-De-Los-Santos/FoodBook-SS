using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Domain.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FoodBook_SS.Infrastructure.Adapters
{

    public class StripePaymentAdapter : IPaymentGateway
    {
        private readonly IConfiguration _config;
        private readonly ILogger<StripePaymentAdapter> _logger;

        public StripePaymentAdapter(IConfiguration config, ILogger<StripePaymentAdapter> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<GatewayResult> ProcesarPagoAsync(string referencia, decimal monto, string metodo)
        {
            try
            {

                _logger.LogInformation("Procesando pago Stripe | Ref: {Ref} | Monto: {Monto}", referencia, monto);
                await Task.CompletedTask;


                return new GatewayResult(true, $"STRIPE-{Guid.NewGuid():N}"[..16], "Aprobado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en gateway Stripe");
                return new GatewayResult(false, null, ex.Message);
            }
        }
    }
}
