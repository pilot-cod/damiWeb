using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace damiWeb.Models
{
    [Table("tblOrderDetail")]
    public class OrderDetail
    {
        [Key] public int RowID { get; set; }
        public int OrderID { get; set; }
        public int LineNumber { get; set; }
        public string ItemID { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }

        public OrderMaster? OrderMaster { get; set; }
        public Item? Item { get; set; }
    }
}
