using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;

namespace RabbitMQPractices.WatermarkerWebApp.Services
{
    public class RabbitMQClientService:IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IChannel _channel;
        public static string ExchangeName = "DirectImageExchange";
        public static string WatermarkRoute = "watermark-route";
        public static string QueueName = "queue-watermark-image";

        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<IChannel> Connect()
        {
            _connection = _connectionFactory.CreateConnectionAsync().Result;
            if (_channel is {IsOpen : true })
            {
                return _channel;
            }

            _channel = _connection.CreateChannelAsync().Result;

            await _channel.ExchangeDeclareAsync(ExchangeName, type: ExchangeType.Direct, true, false);
            await _channel.QueueDeclareAsync(QueueName, true, false, false,null);
            await _channel.QueueBindAsync(exchange:ExchangeName,queue:QueueName,routingKey:WatermarkRoute);


            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu...");

            return _channel;
        }

        public void Dispose()
        {
            _channel?.CloseAsync();
            _channel?.Dispose();
            _connection?.CloseAsync();
            _connection?.Dispose();

            _logger.LogInformation("RabbitMQ Bağlantısı kapatıldı...");
        }
    }
}
