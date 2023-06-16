using System.ComponentModel.DataAnnotations;
namespace FNWeb.Models
{
    public class Product_Cart
    {
        [Key]
        public string ID_PC { get; set; }
        public string ID_User { get; set; }
        public string ID_Product { get; set; }
        public int Quantity { get; set; }
    }
}
