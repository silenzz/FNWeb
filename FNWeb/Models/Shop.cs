using System.ComponentModel.DataAnnotations;

namespace FNWeb.Models
{
    public class Shop
    {
        [Key]
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
