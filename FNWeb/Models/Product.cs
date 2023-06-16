using System.ComponentModel.DataAnnotations;

namespace FNWeb.Models
{
    public class Product
    {
        [Key]
        public string ID_Product { get; set; }
        public string Name { get; set; }
        public Double Price { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Type_product { get; set; }
        public string ID_Shop { get; set; }
        
    }
}
