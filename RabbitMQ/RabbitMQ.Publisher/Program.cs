using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
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
            //SimpleLevelPublisher();

            //Direct Exchange Publisher Örneği
            //SimpleDirectExchangeLevelPublisher();

            //Fanout Exchange Publisher Örneği
            //SimpleFanoutExchangeLevelPublisher();

            //Topic Exchange Publisher Örneği
            //SimpleTopicExchangeLevelPublisher();

            //Header Exchange Publisher Örneği
            //SimpleHeaderExchangeLevelPublisher();
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

            channel.QueueDeclare(queue: "example-queue", exclusive: false,durable:true); //exclusive yapmamızın nedeni birden fazla bağlantıyla bu kuyrukta işlem yapıp yapamayacağımızı belirtmek için.

            //Queue mesaj gönderme

            //NOT! : Rabbitmq kuyruğa atacağı mesajları byte türünden kabul etmektedir. Haliyle mesajları byte'e dönüştürmemiz gerekiyor.

            #region burasını şimdilik yorum satırı yaptım.
            //byte[] message = Encoding.UTF8.GetBytes("Merhaba !");

            //channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message);
            #endregion

            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            for (int i = 0; i < 100; i++)
            {
                Task.Delay(200); // bekletme

                byte[] message = Encoding.UTF8.GetBytes("Merhaba "+i);

                channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message,basicProperties:properties);
            }

            Console.Read();
        }
        public static void SimpleDirectExchangeLevelPublisher()
        {

            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("*****");


            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "direct-exchange-example", type: ExchangeType.Direct);

            while (true)
            {
                Console.Write("Mesaj : ");
                string message = Console.ReadLine();
                byte[] byteMessage = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "direct-exchange-example", routingKey: "direct-queue-example", body: byteMessage);
            }

            Console.Read();

        }
        public static async void SimpleFanoutExchangeLevelPublisher()
        {

            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("******");


            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "fanout-exchange-example", type: ExchangeType.Fanout);

            for (int i = 0; i < 100; i++)
            {
                Task.Delay(200);
                byte[] message = Encoding.UTF8.GetBytes("Merhaba : " + i);

                channel.BasicPublish(exchange: "fanout-exchange-example",routingKey:string.Empty,body:message);
            }

          
            Console.Read();



        }
        public static void SimpleTopicExchangeLevelPublisher()
        {

            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("******");


            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange:"topic-example-exchange",type:ExchangeType.Topic);

            for (int i = 0; i < 100; i++)
            {
                Task.Delay(200);
                byte[] message = Encoding.UTF8.GetBytes("Merhaba + " +i);
                Console.Write("Mesajın gönderileceği topic formatını belirtiniz :");
                string topic = Console.ReadLine();
                channel.BasicPublish(exchange: "topic-example-exchange", routingKey: topic, body: message);
            }
            Console.Read();

        }
        public static void SimpleHeadercExchangeLevelPublisher()
        {

            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("********");


            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange:"header-exchange-example", type: ExchangeType.Headers);

            for (int i = 0; i < 100; i++)
            {
                Task.Delay(200);
                byte[] message = Encoding.UTF8.GetBytes("Merhaba" + i);
                Console.Write("Lütfen kullanılacak header value'sini giriniz :");
                string value = Console.ReadLine();

               IBasicProperties basicProperties = channel.CreateBasicProperties();
                basicProperties.Headers = new Dictionary<string,object>()
                {
                    ["no"] = value
                };

                channel.BasicPublish(exchange: "header-exchange-example", routingKey: String.Empty, body: message,basicProperties:basicProperties);

            }

            Console.Read();

        }


        #region message design patternler
        public static void PointToPointDesignExamplePublisher()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("********");


            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();


            string queneName = "example-p2p-queue";
            channel.QueueDeclare(queue: queneName, durable: false, exclusive: false, autoDelete: false);

            byte[] message = Encoding.UTF8.GetBytes("Merhaba");

            channel.BasicPublish(exchange: string.Empty, routingKey: queneName, body: message);
        }

        public static void PublishSubsribeDesignExampleConsumer()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("********");

            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            string exchangeName = "example-pub-sub-queue";
            channel.ExchangeDeclare(exchange:exchangeName,type:ExchangeType.Fanout);

            for (int i = 0; i < 100; i++)
            {
                Task.Delay(200);
                byte[] message = Encoding.UTF8.GetBytes("Merhaba " + i);

                channel.BasicPublish(exchange: exchangeName, routingKey: string.Empty, body: message);
            }
          
        }

        public static void WorkQueuneDesignExampleConsumer()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("********");

            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            string queueName = "example-work-queue";

            channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false);

            for (int i = 0; i < 100; i++)
            {
                Task.Delay(200);
                byte[] message = Encoding.UTF8.GetBytes("Merhaba " + i);

                channel.BasicPublish(exchange: string.Empty, routingKey: string.Empty, body: message);
            }

        }

        public static void RequestResponseDesignExampleConsumer()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("********");

            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            string requestQueueName = "example-request-response-queue";
            channel.QueueDeclare(
                queue: requestQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false);

            string replyQueuneName = channel.QueueDeclare().QueueName;

            string correlationId = Guid.NewGuid().ToString();

            #region Request mesajını oluşturma ve gönderme

            IBasicProperties basicProperties = channel.CreateBasicProperties();
            basicProperties.CorrelationId = correlationId;
            basicProperties.ReplyTo = replyQueuneName;

            for (int i = 0; i < 100; i++)
            {
                Task.Delay(200);
                byte[] message = Encoding.UTF8.GetBytes("Merhaba " + i);

                channel.BasicPublish(exchange: string.Empty, routingKey: requestQueueName, body: message,basicProperties:basicProperties);
            }

            #endregion

            #region Response kuyruğunu dinleme

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue:replyQueuneName,autoAck:true,consumer:consumer);

            consumer.Received += (sender, e) =>
            {
                if (e.BasicProperties.CorrelationId == correlationId)
                {
                    string message = Encoding.UTF8.GetString(e.Body.ToArray());
                    Console.WriteLine("Response :" + message);
                }
            };

            #endregion

        }

        #endregion


    }
}
