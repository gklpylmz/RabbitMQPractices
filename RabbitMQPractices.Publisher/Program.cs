using RabbitMQ.Client;
using RabbitMQPractices.Publisher;
using Shared;
using System.Text;
using System.Text.Json;

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
    Persistent = true,
};

var product = new Product()
{
    Id = 1,
    Name = "Defter",
    Price= 100 ,
    Stock= 10,
};
var productJson = JsonSerializer.Serialize(product);

//await channel.BasicPublishAsync("header-exchange", string.Empty,
//    Encoding.UTF8.GetBytes(productJson),basicProperties:properties);

Console.WriteLine("Mesaj Gönderilmiştir.");

Console.ReadLine();
