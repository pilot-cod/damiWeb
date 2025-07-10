using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace damiWeb.Models
{
    [Table("tblItemList")]
    public class Item
    {
        [Key] public int ItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string InvUnitOfMeasr { get; set; } = string.Empty;
    }
}
