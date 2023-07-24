using MassTransit;
using RabbitMQ.MassTransit.Shared.Messages;

// RabbitMQ'ya bağlantı oluştur

string rabbitMQUri = "amqps://lqklsdpn:EWvh4S5aLfwruWutVwkbkeVik5OI6mkw@toad.rmq.cloudamqp.com/lqklsdpn";

string queueName = "example-queue";

IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri);
});


// Gönderilecek mesajın tipini ve içeriğini belirle
ISendEndpoint sendEndpoint = await bus.GetSendEndpoint(new($"{rabbitMQUri}/{queueName}"));

// Kullanıcıdan mesaj al ve mesajı belirlenen kuyruğa gönder
Console.Write("Gönderilecek mesaj:");
string message = Console.ReadLine();
sendEndpoint.Send<IMessage>(new ExampleMessage() // IMassTransitMessage arayüzünü uygulayan ExampleMessage tipinde bir nesne oluşturup gönderilecek mesajı içerisine yerleştirir.
{
    Text = message
});
Console.Read();