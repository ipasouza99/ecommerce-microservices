using System;
using  Microsoft.AspNetCore.Mvc;
using Sales.Data;
using Sales.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Sales.DTOs;
using Sales.DtOs;
using System.Linq.Expressions;
using Sales.Services;


namespace Sales.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class OrderController : ControllerBase
    {
        private readonly SalesDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        private readonly IMessageProducer _messageProducer;
        public OrderController(SalesDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration, IMessageProducer messageProducer)

        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _messageProducer = messageProducer;
        }
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderRequestDto orderDto)
        {
            var client = _httpClientFactory.CreateClient();
            var newOrderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            var inventoryServiceUrl = _configuration["ServiceUrls:InventoryApi"];
            foreach (var itemDto in orderDto.OrderItems)
            {
                ProductDto? product;
                try
                {
                    product = await client.GetFromJsonAsync<ProductDto>($"{inventoryServiceUrl}/api/products/{itemDto.ProductId}");

                }
                catch (HttpRequestException)
                {
                    return BadRequest($"Não foi possível conectar ao serviço de estoque.");
                }
                if (product == null)
                {
                    return BadRequest($"Produto com ID {itemDto.ProductId} não encontrado.");
                }
                if (product.StockQuantity < itemDto.Quantity)
                {
                    return BadRequest($"Estoque insuficiente para o produto {product.Name}. Estoque disponível: {product.StockQuantity}, Quantidade solicitada: {itemDto.Quantity}");

                }
                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                };
                newOrderItems.Add(orderItem);
                totalAmount += orderItem.UnitPrice * orderItem.Quantity;

            }
            var newOrder = new Order
            {
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                OrderItems = newOrderItems
            };
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();
            _messageProducer.SendMessage(newOrder);

            return CreatedAtAction(nameof(GetOrder), new { id = newOrder.Id }, newOrder);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
    }
}

