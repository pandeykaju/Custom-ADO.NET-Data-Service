
using System.ComponentModel.DataAnnotations;

namespace CustomADODataService.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
