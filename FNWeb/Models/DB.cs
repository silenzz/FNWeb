using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FNWeb.Models
{
    public class DB : DbContext
    {
        public DB(DbContextOptions<DB> options)
              : base(options)
        { }
        public DbSet<Account> Account { get; set; }
        public DbSet<User_information> User_information { get; set; }
        public DbSet<Shop> Shop { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<OTP> OTP { get; set; }
 
        public DbSet<Product_Cart> Product_Cart { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Product_Order> Product_Order { get; set; }
    }
}
