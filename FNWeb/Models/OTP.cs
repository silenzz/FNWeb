using System.ComponentModel.DataAnnotations;

namespace FNWeb.Models
{
    public class OTP
    {
        [Key]
        public string Email { get; set; }
        public DateTime Time_end { get; set; }
        public int otp { get; set; }
    }
}
