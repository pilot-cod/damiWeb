using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace damiWeb.Models
{
    [Table("tblCustomerList")]
    public class Customer
    {
        [Key] public string CustomerID { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? TaxCode { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
