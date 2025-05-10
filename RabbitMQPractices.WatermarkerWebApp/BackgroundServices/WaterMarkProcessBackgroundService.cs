
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQPractices.WatermarkerWebApp.Services;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace RabbitMQPractices.WatermarkerWebApp.BackgroundServices
{
    public class WaterMarkProcessBackgroundService : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitmqClientService;
        private readonly ILogger<WaterMarkProcessBackgroundService> _logger;
        private IChannel _channel; 

        public WaterMarkProcessBackgroundService(RabbitMQClientService rabbitmqClientService, ILogger<WaterMarkProcessBackgroundService> logger)
        {
            _rabbitmqClientService = rabbitmqClientService;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitmqClientService.Connect().Result;

            _channel.BasicQosAsync(0, 1, false); 

            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.BasicConsumeAsync(RabbitMQClientService.QueueName,false,consumer);

            //consumer.ReceivedAsync += (sender, @event) => { }

            consumer.ReceivedAsync += Consumer_ReceivedAsync;

            return Task.CompletedTask;

        }

        private Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                var productImageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", productImageCreatedEvent.ImageUrl);

                var markString = "www.cemalgokalpyilmaz.com";
                using var img = Image.FromFile(path);

                using var graphic = Graphics.FromImage(img);

                var font = new Font(FontFamily.GenericMonospace, 32, FontStyle.Bold, GraphicsUnit.Pixel);
                var textSize = graphic.MeasureString(markString, font);
                var color = Color.FromArgb(255, 255, 255, 255);
                var brush = new SolidBrush(color);

                var position = new Point(img.Width - ((int)textSize.Width + 30), img.Height - ((int)textSize.Height + 30));

                graphic.DrawString(markString, font, brush, position);

                img.Save("wwwroot/Images/watermarks/" + productImageCreatedEvent.ImageUrl);

                img.Dispose();
                graphic.Dispose();

                _channel.BasicAckAsync(@event.DeliveryTag,false);
            }
            catch (Exception ex)
            {

                _logger.LogInformation(ex.Message);
            }
            return Task.CompletedTask;
        
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
