using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMQPractices.WatermarkerWebApp.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitmqClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitmqClientService)
        {
            _rabbitmqClientService = rabbitmqClientService;
        }

        public void Publish(ProductImageCreatedEvent productImageCreatedEvent)
        {
            var channel = _rabbitmqClientService.Connect().Result;

            var bodyString = JsonSerializer.Serialize(productImageCreatedEvent);
            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var props = new BasicProperties()
            {
                Persistent = true
            };

            channel.BasicPublishAsync(
                exchange: RabbitMQClientService.ExchangeName,
                routingKey: RabbitMQClientService.WatermarkRoute,
                basicProperties: props,
                body: bodyByte,
                mandatory:false
             );




        }
    }
}
