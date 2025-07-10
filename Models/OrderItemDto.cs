namespace damiWeb.Models
{
    public class OrderItemDto
    {
        public string ItemID { get; set; } = string.Empty;
        public string? ItemName { get; set; }
        public string? Unit { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount => Quantity * Price;
    }
}
