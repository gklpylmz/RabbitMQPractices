using RabbitMQ.Client;
using RabbitMQPractices.Publisher;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://localhost:5672");


var connection = await factory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();

//Durable :true exchange kaybolmaması için.
await channel.ExchangeDeclareAsync("logs-topic", durable: true, type: ExchangeType.Topic);

Random rnd = new Random();
Enumerable.Range(1, 50).ToList().ForEach(x =>
 {
     LogNames log1 = (LogNames)rnd.Next(1, 5);
     LogNames log2 = (LogNames)rnd.Next(1, 5);
     LogNames log3 = (LogNames)rnd.Next(1, 5);

     string message = $"Log-Type : {log1}-{log2}-{log3}";
     var messageBody = Encoding.UTF8.GetBytes(message);
     var routeKey = $"{log1}.{log2}.{log3}";

     channel.BasicPublishAsync("logs-topic", routeKey,messageBody);

     Console.WriteLine($"Log Gönderilmiştir. {message}");
 });


Console.ReadLine();
