using System.ComponentModel.DataAnnotations;

namespace FNWeb.Models
{
    public class Orders
    {
        [Key]
        public string ID_Order { get; set; }
        public string ID_User { get; set; }
        public string ID_Shop { get; set; }
        public string Address_ship { get; set;}
        public double Price { get; set;}
        public DateTime DateAt { get; set;}
        public int Status { get; set; }
    }
}
