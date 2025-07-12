using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace damiWeb.Models
{
    [Table("tblOrderDetail")]
    public class OrderDetail
    {
        [Key] public long RowID { get; set; }
        public long OrderID { get; set; }
        public int LineNumber { get; set; }
        public string ItemID { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }

        [ForeignKey("OrderID")]
        public OrderMaster? OrderMaster { get; set; }
    }
}
