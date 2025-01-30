using System.Text.Json.Serialization;

namespace TillApp.Shared.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }

    }
}
