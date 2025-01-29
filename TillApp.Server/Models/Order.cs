namespace TillApp.Server.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public string OrderName { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
