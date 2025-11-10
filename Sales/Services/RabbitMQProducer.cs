using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Sales.Services
{
    public class RabbitMQProducer : IMessageProducer        
    {
        public void SendMessage<T>(T message)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
               channel.QueueDeclare(queue: "orders",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var jsonString = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(jsonString);
               
                channel.BasicPublish(exchange: "",
                                     routingKey: "orders", 
                                     basicProperties: null,
                                     body: body);
            }
        }       
    }
}