using System.ComponentModel.DataAnnotations;

namespace FNWeb.Models
{
    public class Product_Order
    {
        [Key]
        public string ID_PO { get; set; }
        public string ID_Order { get; set; }
        public string ID_Product { get; set; }
        public int Quantity { get; set; }
    }
}
   