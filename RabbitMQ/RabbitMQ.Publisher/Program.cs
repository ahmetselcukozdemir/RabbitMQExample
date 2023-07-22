using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //basit düzeyde bir publisher örneği
            SimpleLevelPublisher();

        }

        public static void SimpleLevelPublisher()
        {
            //öncelikle bir bağlantı oluşturuyoruz.

            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("*****************");

            //bağlantıyı aktifleştirme ve kanal açma

            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            //Queue oluşturma(kuyruk oluşturma) 

            channel.QueueDeclare(queue: "example-queue", exclusive: false); //exclusive yapmamızın nedeni birden fazla bağlantıyla bu kuyrukta işlem yapıp yapamayacağımızı belirtmek için.

            //Queue mesaj gönderme

            //NOT! : Rabbitmq kuyruğa atacağı mesajları byte türünden kabul etmektedir. Haliyle mesajları byte'e dönüştürmemiz gerekiyor.

            #region burasını şimdilik yorum satırı yaptım.
            //byte[] message = Encoding.UTF8.GetBytes("Merhaba !");

            //channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message);
            #endregion

            for (int i = 0; i < 100; i++)
            {
                Task.Delay(200); // bekletme

                byte[] message = Encoding.UTF8.GetBytes("Merhaba "+i);

                channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message);
            }

            Console.Read();
        }
    }
}
