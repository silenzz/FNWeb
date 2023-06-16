using System.ComponentModel.DataAnnotations;

namespace FNWeb.Models
{
    public class User_information
    {
        [Key]
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
