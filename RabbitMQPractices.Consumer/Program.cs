using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://localhost:5672");


var connection = await factory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();



await channel.BasicQosAsync(0, 1, false);
var consumer = new AsyncEventingBasicConsumer(channel);

var queueName = channel.QueueDeclareAsync().Result.QueueName;
//"Info.#
//. Tek bir karakteri ifade ediyorken # birden fazla karaketeri ifade eder.
var routeKey = "*.*.Warning";
await channel.QueueBindAsync(queueName, "logs-topic", routeKey);

Console.WriteLine("Loglar dinleniyor");

consumer.ReceivedAsync += async (object sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());

    Thread.Sleep(1500);

    Console.WriteLine("Gelen Mesaj : " + message);

    File.AppendAllText("log-critical.txt", message + "\n");

    await channel.BasicAckAsync(e.DeliveryTag, false);
};
await channel.BasicConsumeAsync(queueName, false, consumer);
Console.ReadLine();


