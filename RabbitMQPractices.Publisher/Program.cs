using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://localhost:5672");


using (var connection = await factory.CreateConnectionAsync())
{
    var channel = await connection.CreateChannelAsync();


    //Durable :true exchange kaybolmaması için.
    await channel.ExchangeDeclareAsync("logs-fanout",durable:true,type:ExchangeType.Fanout);

    Enumerable.Range(1, 50).ToList().ForEach(x =>
    {
        string message = $"Log {x}";

        var messageBody = Encoding.UTF8.GetBytes(message);

        channel.BasicPublishAsync("logs-fanout","", messageBody);

        Console.WriteLine($"Mesaj Gönderilmiştir. {message}");
    });

   
    Console.ReadLine();
}