﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Reflection;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://localhost:5672");


var connection = await factory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();



await channel.BasicQosAsync(0, 1, false);
var consumer = new AsyncEventingBasicConsumer(channel);

var queueName = channel.QueueDeclareAsync().Result.QueueName;

Dictionary<string, object> headers = new Dictionary<string, object>();
headers.Add("format", "pdf");
headers.Add("shape", "a");
headers.Add("x-match", "any");
await channel.QueueBindAsync(queueName, "header-exchange",string.Empty,headers);

Console.WriteLine("Loglar dinleniyor");

consumer.ReceivedAsync += async (object sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Product p = JsonSerializer.Deserialize<Product>(message);

    Thread.Sleep(1500);

    Console.WriteLine("Gelen Mesaj : " + p.Id + " " +p.Name + " " + p.Price + " " + p.Stock);


    await channel.BasicAckAsync(e.DeliveryTag, false);
};
await channel.BasicConsumeAsync(queueName, false, consumer);
Console.ReadLine();


