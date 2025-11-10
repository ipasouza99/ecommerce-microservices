using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Sales.DTOs
{
    public class OrderItemDTO
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Range(1, 100, ErrorMessage = "Quantidade deve ser entre 1 e 100")]
        public int Quantity { get; set; }

    }
}

