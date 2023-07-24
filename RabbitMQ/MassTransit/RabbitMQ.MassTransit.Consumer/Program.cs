using MassTransit;
using RabbitMQ.MassTransit.Consumer.Consumers;
using RabbitMQ.MassTransit.Shared.Messages;

string rabbitMQUri = "amqps://lqklsdpn:EWvh4S5aLfwruWutVwkbkeVik5OI6mkw@toad.rmq.cloudamqp.com/lqklsdpn";

string queueName = "example-queue"; // kuyruk adı

// RabbitMQ'ya bağlantı oluştur ve mesajları alacak bir endpoint oluştur
IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri); // Belirtilen RabbitMQ adresine bağlanmak için Host metodu kullanılır

    // Belirtilen kuyruk adını dinlemek için bir ReceiveEndpoint oluştur
    factory.ReceiveEndpoint(queueName: queueName, enpoint =>
    {
        enpoint.Consumer<ExampleMessageConsumer>(); // Alınan mesajları işleyecek olan ExampleMessageConsumer tüketici sınıfını endpoint'e ekle
    });
});

await bus.StartAsync();

Console.Read();


