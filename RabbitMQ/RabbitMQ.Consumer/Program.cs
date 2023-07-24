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

            //SimpleLevelConsumer();

            //Direct Exchange Consumer Örneği
            //SimpleDirectExchangeLevelConsumer();

            //Fanout Exchange Consumer Örneği
            //SimpleFanoutExchangeLevelConsumer();

            //Topic Exchange Consumer Örneği
            //SimpleTopicExchangeLevelConsumer();

            //Header Exchange Consumer Örneği
            //SimpleHeaderExchangeLevelConsumer();
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

            //channel.BasicConsume(queue: "example-queue",false,consumer);
            var consumerTag = channel.BasicConsume(queue: "example-queue",autoAck:false,consumer:consumer);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
           
            consumer.Received += (sender, e) =>
            {
                //kuyruğa gelen mesajın işlendiği yerdir.
                //e.Body : bize kuyruktaki mesajın verisini bütünsel olarak getirecektir.
                //e.Body.Span veya e.Body.ToArray() : kuyruktaki verinin byte verisini getirecektir.

                byte[] bodyData = e.Body.ToArray();
                Console.WriteLine(Encoding.UTF8.GetString(bodyData));
                channel.BasicAck(deliveryTag:e.DeliveryTag,multiple:false);
            };

            Console.Read();
        }

        public static void SimpleDirectExchangeLevelConsumer()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("*******");

            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "direct-exchange-example", type: ExchangeType.Direct);
            //1.Adım : Publisher'daki exchange ile aynı isim ve type'a sahip bir exchange tanımlanmalıdır.

            string queueName = channel.QueueDeclare().QueueName;

            //2. Adım : Publisher tarafından routing key'de bulunan değerdeki kuyruğa gönderilen mesajları kendi oluşturduğumuz kuyruğa yönlendirerek tüketmemiz gerekmektedir. Bunu içinde öncelikle bir kuyruk oluşturulmalıdır.

            channel.QueueBind(queue: queueName, exchange: "direct-exchange-example", routingKey: "direct-queue-example");

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            consumer.Received += (sender, e) =>
            {
                byte[] bodyData = e.Body.ToArray();
                Console.WriteLine(Encoding.UTF8.GetString(bodyData));
            };

            Console.Read();
        }

        public static void SimpleFanoutExchangeLevelConsumer()
        {

            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("*************");

            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "fanout-exchange-example", type: ExchangeType.Fanout);

            Console.Write("Kuyruk Adını giriniz :");
            string queueName = Console.ReadLine();

            channel.QueueDeclare(queue: queueName, exclusive: false);

            channel.QueueBind(queue:queueName,exchange: "fanout-exchange-example",routingKey:String.Empty);

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue:queueName,autoAck:true,consumer: consumer);

            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());

                Console.WriteLine(message);
            };

            Console.Read();
        }

        public static void SimpleTopicExchangeLevelConsumer()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("*******");

            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "topic-example-exchange", type: ExchangeType.Topic);

            Console.Write("Dinlenecek topic formatını belirtiniz :");
            string topic = Console.ReadLine();

            string queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue:queueName,exchange: "topic-example-exchange",routingKey:topic);

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue:queueName,autoAck:true,consumer:consumer);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine(message);
            };
            Console.Read();
        }

        public static void SimpleHeaderExchangeLevelConsumer()
        {

            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("********");

            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "header-exchange-example", type: ExchangeType.Headers);

            Console.Write("Lütfen header value'sini giriniz :");
            string value = Console.ReadLine();

            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName, exchange: ExchangeType.Headers, routingKey: String.Empty, new Dictionary<string, object>()
            {
                ["no"] = value
            });

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine(message);
            };

            Console.Read();
        }






        #region message design patternler

        public static void PointToPointDesignExampleConsumer()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("********");


            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();


            string queneName = "example-p2p-queue";
            channel.QueueDeclare(queue: queneName, durable: false, exclusive: false, autoDelete: false);

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: queneName, autoAck: false, consumer: consumer);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine(message);
            };
        }

        public static void PublishSubsribeDesignExampleConsumer()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("********");


            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();


            string exchangeName = "example-pub-sub-queue";
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

            string queneName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queneName, exchange: exchangeName, routingKey: string.Empty);

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: queneName, autoAck: false, consumer: consumer);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine(message);
            };
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

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: queueName, autoAck: true,consumer: consumer);

            channel.BasicQos(prefetchCount: 1, prefetchSize: 0, global: false);

            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine(message);
            };
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

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(
                queue: requestQueueName,
                autoAck: true,
                consumer: consumer);

            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine(message);
                //.....
                byte[] responseMessage = Encoding.UTF8.GetBytes($"İşlem tamamlandı. : {message}");
                IBasicProperties properties = channel.CreateBasicProperties();
                properties.CorrelationId = e.BasicProperties.CorrelationId;
                channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: e.BasicProperties.ReplyTo,
                    basicProperties: properties,
                    body: responseMessage);
            };
        }

        #endregion


    }
}
