namespace Inventory.DTOs
{
    public class OrderMessageDto
    {
         public int Id { get; set; }
        public List<OrderItemMessageDto> OrderItems { get; set; } = new();
    }
}

