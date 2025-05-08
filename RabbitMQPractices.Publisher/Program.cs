using RabbitMQ.Client;
using RabbitMQPractices.Publisher;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://localhost:5672");


var connection = await factory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();

//Durable :true exchange kaybolmaması için.
await channel.ExchangeDeclareAsync("logs-direct", durable: true, type: ExchangeType.Direct);

Enum.GetNames(typeof(LogNames)).ToList().ForEach(async x =>
{
    var routeKey = $"route-{x}";
    var queueName = $"direct-queue-{x}";
    await channel.QueueDeclareAsync(queueName, true, false, false);

    await channel.QueueBindAsync(queueName, "logs-direct", routeKey,null);
});

Enumerable.Range(1, 50).ToList().ForEach(x =>
 {
     LogNames log = (LogNames)new Random().Next(1, 5);
     string message = $"Log-Type : {log}";

     var messageBody = Encoding.UTF8.GetBytes(message);

     var routeKey = $"route-{log}";

     channel.BasicPublishAsync("logs-direct", routeKey,messageBody);

     Console.WriteLine($"Log Gönderilmiştir. {message}");
 });


Console.ReadLine();
