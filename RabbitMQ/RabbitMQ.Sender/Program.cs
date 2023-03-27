using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Sender
{
    internal class Program
    {
        static void Main(string[] args)
        {

            for (int i = 0; i < 10; i++)
            {
                SenderMessage();
            }

        }

        public static void SenderMessage()
        {
            try
            {
                var factory = new ConnectionFactory();

                //url'i tanımlıyoruz.
                factory.Uri = new Uri("amqps://txuwvldz:4SmUuZQISipyjCoSrM81QFEhudCpZE9N@chimpanzee.rmq.cloudamqp.com/txuwvldz");

                //connection oluşturuyoruz.

                var connection = factory.CreateConnection();

                //connection'na bir kanal yaratıyoruz.
                var channel = connection.CreateModel();

                channel.QueueDeclare("mesaj-kuyruk", true, false, false);

                //gönderilecek mesajı tanımlıyoruz.

                var mesaj = "Test";

                //mesajı kuyruğa iletebilmek için binary formatına çeviriyoruz.

                var body = Encoding.UTF8.GetBytes(mesaj);

                channel.BasicPublish(String.Empty, "mesaj-kuyruk", null, body);

                Console.WriteLine("Mesaj kuyruğa eklendi.");

                Console.ReadLine();
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
