using FoodBook_SS.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FoodBook_SS.Infrastructure.Adapters
{

    public class SmsNotificationAdapter : INotificationSender
    {
        private readonly ILogger<SmsNotificationAdapter> _logger;

        public SmsNotificationAdapter(ILogger<SmsNotificationAdapter> logger) => _logger = logger;

        public async Task EnviarAsync(string destinatario, string asunto, string mensaje)
        {
            try
            {

                _logger.LogInformation("SMS enviado a {Destinatario}", destinatario);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar SMS a {Destinatario}", destinatario);
                throw;
            }
        }
    }
}