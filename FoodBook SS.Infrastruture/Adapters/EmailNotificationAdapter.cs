using FoodBook_SS.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FoodBook_SS.Infrastructure.Adapters
{

    public class EmailNotificationAdapter : INotificationSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailNotificationAdapter> _logger;

        public EmailNotificationAdapter(IConfiguration config, ILogger<EmailNotificationAdapter> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task EnviarAsync(string destinatario, string asunto, string mensaje)
        {
            try
            {

                _logger.LogInformation("Email enviado a {Destinatario} | Asunto: {Asunto}", destinatario, asunto);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email a {Destinatario}", destinatario);
                throw;
            }
        }
    }
}

