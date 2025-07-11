using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace damiWeb.Models
{
    [Table("tblItemList")]
    public class Item
    {
        [Key] public string ItemID { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string InvUnitOfMeasr { get; set; } = string.Empty;
    }
}
