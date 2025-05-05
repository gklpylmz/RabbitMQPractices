using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://localhost:5672");


using (var connection = await factory.CreateConnectionAsync())
{
    var channel = await connection.CreateChannelAsync();
    //await channel.QueueDeclareAsync("hello-queue", true, false, false);

    await channel.BasicQosAsync(0, 1, false);
    var consumer = new AsyncEventingBasicConsumer(channel);

    await channel.BasicConsumeAsync("hello-queue",false,consumer);

    consumer.ReceivedAsync += async (object sender, BasicDeliverEventArgs e) =>
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());

        Thread.Sleep(1500);

        Console.WriteLine("Gelen Mesaj : " +message);

        await channel.BasicAckAsync(e.DeliveryTag,false);
    };

    Console.ReadLine();
   
}
