using System.ComponentModel.DataAnnotations;

namespace Sales.DTOs
{
    public class CreateOrderRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "O pedido deverá conter pelo menos um item.")]
        public List<OrderItemDTO> OrderItems { get; set; }
    }
}