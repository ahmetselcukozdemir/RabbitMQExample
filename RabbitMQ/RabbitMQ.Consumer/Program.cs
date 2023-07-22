using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //BASİT DÜZEYDE BİR CONSUMER ÖRNEĞİ

            SimpleLevelConsumer();
        }

        public static void SimpleLevelConsumer()
        {
            //öncelikle bir bağlantı oluşturuyoruz.

            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("***********");

            //bağlantıyı aktifleştirme ve kanal açma

            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            //Queue oluşturma(kuyruk oluşturma) 

            channel.QueueDeclare(queue: "example-queue", exclusive: false); //ÖNEMLİ ! Consumerdaki kuyruk pusblisherdaki gibi bire bir aynı tanımlanmalıdır.

            //Queue(kuyruktan) mesaj okuma

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: "example-queue",false,consumer);
            consumer.Received += (sender, e) =>
            {
                //kuyruğa gelen mesajın işlendiği yerdir.
                //e.Body : bize kuyruktaki mesajın verisini bütünsel olarak getirecektir.
                //e.Body.Span veya e.Body.ToArray() : kuyruktaki verinin byte verisini getirecektir.

                byte[] bodyData = e.Body.ToArray();
                Console.WriteLine(Encoding.UTF8.GetString(bodyData));
            };

            Console.Read();
        }
    }
}
