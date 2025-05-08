using RabbitMQ.Client;
using RabbitMQPractices.Publisher;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://localhost:5672");


var connection = await factory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();

//Durable :true exchange kaybolmaması için.
await channel.ExchangeDeclareAsync("header-exchange", durable: true, type: ExchangeType.Headers);

Dictionary<string,object> headers = new Dictionary<string, object>();
headers.Add("format", "pdf");
headers.Add("shape", "a");

var properties = new BasicProperties()
{
    Headers = headers,
};

//await channel.BasicPublishAsync("header-exchange", string.Empty,
//    Encoding.UTF8.GetBytes("header mesajım"),basicProperties:properties);

Console.WriteLine("Mesaj Gönderilmiştir.");

Console.ReadLine();
