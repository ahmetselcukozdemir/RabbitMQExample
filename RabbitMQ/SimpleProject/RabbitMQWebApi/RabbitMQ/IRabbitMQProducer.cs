namespace RabbitMQWebApi.RabbitMQ
{
    public interface IRabbitMQProducer
    {
        public void SendProductMessage<T>(T message);
    }
}
