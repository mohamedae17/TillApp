namespace TillApp.Server.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public Order Order { get; set; }
    }
}
