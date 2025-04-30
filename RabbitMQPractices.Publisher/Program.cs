using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://localhost:5672");


using (var connection = await factory.CreateConnectionAsync())
{
    var channel = await connection.CreateChannelAsync();
    await channel.QueueDeclareAsync("hello-queue",true,false,false);


    string message = "Hello World";

    var messageBody = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(string.Empty, "hello-queue",messageBody);

    Console.WriteLine("Mesaj Gönderilmiştir.");
    Console.ReadLine();
}