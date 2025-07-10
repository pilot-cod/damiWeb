using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace damiWeb.Models
{
    [Table("tblOrderMaster")]
    public class OrderMaster
    {
        [Key] public int OrderID { get; set; }       
        public DateTime OrderDate { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string CustomerID { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        public Customer? Customer { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
