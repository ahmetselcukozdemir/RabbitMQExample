using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.ESB.MassTransit.Publisher
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string rabbitMQUri = "amqps://befjdvjy:bs5zD-4j8OfHQrZFUOnEAKomCudYmkL1@moose.rmq.cloudamqp.com/befjdvjy";

            string queueName = "example-queue";

            IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
            {
                factory.Host(rabbitMQUri);
            });

            ISendEndpoint sendEndpoint = await bus.GetSendEndpoint(new($"{rabbitMQUri}/{queueName}"));

            Console.Write("Gönderilecek mesaj : ");
            string message = Console.ReadLine();
            await sendEndpoint.Send<IMessage>(new ExampleMessage()
            {
                Text = message
            });

            Console.Read();
        }
    }
}
