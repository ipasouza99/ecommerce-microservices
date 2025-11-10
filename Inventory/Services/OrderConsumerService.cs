using System;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Text.Json;
using Inventory.Data;
using Inventory.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Inventory.Services
{
    public class OrderConsumerService : BackgroundService  
     {
         private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;
        private const string QueueName = "orders"; 

        public OrderConsumerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Mensagem recebida: {message}");

                var orderMessage = JsonSerializer.Deserialize<OrderMessageDto>(message);

                if (await UpdateStock(orderMessage))
                {
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        private async Task<bool> UpdateStock(OrderMessageDto orderMessage)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

                    foreach (var item in orderMessage.OrderItems)
                    {
                        var product = await dbContext.Products.FindAsync(item.ProductId);
                        if (product != null)
                        {
                            product.StockQuantity -= item.Quantity;
                        }
                    }

                    await dbContext.SaveChangesAsync();
                    Console.WriteLine($"[x] Estoque atualizado para o pedido ID: {orderMessage.Id}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Erro ao atualizar o estoque: {ex.Message}");
                return false;
            }
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}

