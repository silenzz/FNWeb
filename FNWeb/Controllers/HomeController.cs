
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using MimeKit;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using FNWeb.Models;

namespace FNWeb.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DB _db;
        private readonly IWebHostEnvironment _env;
        public void SendEmail(string subject, string message1, string rec)
        {
            // Tạo đối tượng SmtpClient và thiết lập các thuộc tính cần thiết
            using (var client = new SmtpClient())
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("anhq6009@gmail.com", "yrrsfdrowpjppsoq");

                // Tạo đối tượng MailMessage và thiết lập các thuộc tính cần thiết
                var message = new MailMessage();
                message.From = new MailAddress("anhq6009@gmail.com");
                message.To.Add(new MailAddress(rec));
                message.Subject = subject;
                message.Body = message1;

                // Gửi email
                client.Send(message);
            }


        }
        public static int GenerateRandomNumber()
        {
            Random rand = new Random();
            return rand.Next(1000, 10000);
        }
        public HomeController(ILogger<HomeController> logger, DB db, IWebHostEnvironment env)
        {
            _logger = logger;
            _db = db;
            _env = env;
        }
        [HttpGet]
        public IActionResult Index()
        {
            string id = HttpContext.Session.GetString("ID");
            string role=HttpContext.Session.GetString("Role");
            if (id != null)
            {
                return Redirect("/"+role);
            }
            return View();
        }

        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}