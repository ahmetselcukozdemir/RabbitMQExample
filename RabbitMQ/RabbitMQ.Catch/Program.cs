using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Catch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri("*********");

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.QueueDeclare("mesaj-kuyruk", true, false, false);

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume("mesaj-kuyruk", true, consumer);

            consumer.Received += Consumer_Received;

            Console.ReadLine();
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine("Gelen mesaj;",Encoding.UTF8.GetString(e.Body.ToArray()));
        }
    }
}
