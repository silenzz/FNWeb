using Microsoft.AspNetCore.Mvc;
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
using System.Data.Entity;
using System.Drawing;
using System.Reflection.Metadata;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Policy;
using Org.BouncyCastle.Crypto;

namespace FNWeb.Controllers
{
    public class UserController : Controller
    {
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
        public UserController(DB db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string ID, string name, string email, string pass)
        {
            _db.Account.Add(new Account { ID = ID, Password = pass, Role = 1 });
            _db.SaveChanges();
            _db.User_information.Add(new User_information { ID = ID, Name = name, Email = email });
            _db.SaveChanges();
            SendEmail($"Welcome {name} with ID={ID}", $"Your account is {ID} with password={pass} ", email);
            ViewData["check"] = "Successful registration";
            return View();
        }
        public IActionResult Index()
        {
            string content = "";
            string check = HttpContext.Session.GetString("ID");

            if (check != null)
            {
                return Redirect("/user/main");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Index(string ID, string password)
        {
            var temp = _db.Account.Find(ID);
            if (temp == null)
            {
                ViewData["check"] = "Tài khoản không tồn tại";
            }
            else
            {
                if (temp.Password.Trim().Equals(password.Trim()))
                {
                    if (temp.Role == 2)
                    {
                        ViewData["check"] = "Chưa có tài khoản này cho user này";
                    }
                    else
                    {
                        HttpContext.Session.SetString("ID", ID);
                        HttpContext.Session.SetString("Role", "User");
                        return Redirect("/user/main");
                    }

                }
                else
                {
                    ViewData["check"] = "Sai mật khẩu";
                }
            }
            return View();
        }
        public IActionResult Main()
        {
            string content = "";
            string check = HttpContext.Session.GetString("ID");

            if (check == null)
            {
                return Redirect("/user");
            }
            foreach (var item in _db.Product)
            {
                if (item.Quantity > 0) {
                    content += $"<div class=\"col-xl-3 col-lg-4 col-md-6 wow fadeInUp\" data-wow-delay=\"0.1s\">" +
                            $"<div class=\"product-item\">" +
                                $"<div class=\"position-relative bg-light overflow-hidden\">" +
                                    $"<img class=\"img-fluid w-100\" src=\"{item.Image}\" alt=\"\">" +
                                $"</div>" +
                                $"<div class=\"text-center p-4\">" +
                                        $"<p class=\"d-block h5 mb-2\" href=\"\">{item.Name}</p>" +
                                        $"<span class=\"text-primary me-1 price\">{item.Price}đ</span><br>" +

                                $"</div>" +
                                $"<div class=\"d-flex border-top\"> " +
                                    $"<small class=\"w-50 text-center border-end py-2\">" +
                                            $"<button type=\"submit\" class=\"shadow btn custom-btn\"><a  style=\"text-decoration:none;\" class=\"text-body\" href=\"/user/detail?ID={item.ID_Product}\">" +
                                            $"<i class=\"fa fa-eye text-primary me-2\"></i>View Detail</a></button></small>" +
                                            $"" +
                                            $"<small class=\"w-50 text-center py-2\">" +
                                                $"<form method=\"post\" action=\"/user/cart1\">" +
                                                    $"<input type=\"hidden\" name=\"product\" value=\"{item.ID_Product}\">" +
                                                    $"<input type=\"hidden\" name=\"amount\" value=\"1\">" +
                                                    $"<button type=\"submit\" class=\"shadow btn custom-btn\">" +
                                                    $"<span style=\"text-decoration:none;\" class=\"text-body\">" +
                                                    $"<i class=\"fa fa-shopping-bag text-primary me-2\"></i>Add to cart</span>" +
                                                    $"</button></form></small></div></div></div>";
                }

            }
            content += $"<script>" +
               $"const priceElements = document.querySelectorAll(\".price\"); " +
               $"priceElements.forEach((element) => {{" +
               $"const price = parseFloat(element.textContent);" +
               "const roundedPrice = price.toLocaleString('vi', {style: 'currency', currency: 'VND' }); " +
               $"element.textContent = roundedPrice;}})" +
               $"</script>";
            ViewData["content"] = content;
            return View();
        }
        public IActionResult Search(string search)
        {
            string content = "";
            string check = HttpContext.Session.GetString("ID");

            if (check == null)
            {
                return Redirect("/user");
            }
            foreach (var item in _db.Product)
            {
                if (item.Quantity > 0)
                {
                    if (item.Name.ToLower().Contains(search.ToLower()))
                    {
                        content += $"<div class=\"col-xl-3 col-lg-4 col-md-6 wow fadeInUp\" data-wow-delay=\"0.1s\">" +
                               $"<div class=\"product-item\">" +
                                   $"<div class=\"position-relative bg-light overflow-hidden\">" +
                                       $"<img class=\"img-fluid w-100\" src=\"{item.Image}\" alt=\"\">" +
                                   $"</div>" +
                                   $"<div class=\"text-center p-4\">" +
                                           $"<p class=\"d-block h5 mb-2\" href=\"\">{item.Name}</p>" +
                                           $"<span class=\"text-primary me-1 price\">{item.Price}đ</span><br>" +

                                   $"</div>" +
                                   $"<div class=\"d-flex border-top\"> " +
                                       $"<small class=\"w-50 text-center border-end py-2\">" +
                                               $"<button type=\"submit\" class=\"shadow btn custom-btn\"><a  style=\"text-decoration:none;\" class=\"text-body\" href=\"/user/detail?ID={item.ID_Product}\">" +
                                               $"<i class=\"fa fa-eye text-primary me-2\"></i>View Detail</a></button></small>" +
                                               $"" +
                                               $"<small class=\"w-50 text-center py-2\">" +
                                                   $"<form method=\"post\" action=\"/user/cart1\">" +
                                                       $"<input type=\"hidden\" name=\"product\" value=\"{item.ID_Product}\">" +
                                                       $"<input type=\"hidden\" name=\"amount\" value=\"1\">" +
                                                       $"<button type=\"submit\" class=\"shadow btn custom-btn\">" +
                                                       $"<span style=\"text-decoration:none;\" class=\"text-body\">" +
                                                       $"<i class=\"fa fa-shopping-bag text-primary me-2\"></i>Add to cart</span>" +
                                                       $"</button></form></small></div></div></div>";
                    }

                }
            }
            content += $"<script>" +
               $"const priceElements = document.querySelectorAll(\".price\"); " +
               $"priceElements.forEach((element) => {{" +
               $"const price = parseFloat(element.textContent);" +
               "const roundedPrice = price.toLocaleString('vi', {style: 'currency', currency: 'VND' }); " +
               $"element.textContent = roundedPrice;}})" +
               $"</script>";
            ViewData["content"] = content;
            return View();
        } 
        public IActionResult Forgot()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Forgot(string email)
        {
            var now = DateTime.Now;
            int x = GenerateRandomNumber();
            var temp = _db.OTP.Find(email);
            if (temp != null)
            {
                _db.OTP.Remove(temp);
                _db.SaveChanges();
            }
            _db.OTP.Add(new OTP { Email = email, Time_end = now.AddMinutes(1), otp = x });
            _db.SaveChanges();
            SendEmail("Forgot password", $"OTP {x}", email);

            return Redirect($"/user/forgot1?email={email}");
        }
        [HttpGet]
        public IActionResult Forgot1(string email)
        {
            ViewData["email"] = email;
            return View();
        }
        [HttpPost]
        public IActionResult Forgot1(string email, int otp)
        {
            var x = _db.OTP.Find(email);
            ViewData["email"] = email;
            if (x != null)
            {
                if (x.otp == otp && DateTime.Compare(DateTime.Now, x.Time_end) < 0)
                {
                    foreach (var item in _db.User_information)
                    {
                        if (item.Email.Trim().Equals(email.Trim()))
                        {
                            var ID_c = _db.Account.Find(item.ID); if (ID_c != null)
                            {
                                SendEmail("Forgot", $"{ID_c.ID} has password={ID_c.Password.Trim()}", email);
                                ViewData["check"] = "Thành công";
                            }
                            break;
                        }
                        else
                        {

                        }
                    }
                }
                else { ViewData["check"] = "OTP sai hoặc hết hạn"; }

            }
            else
            {
                ViewData["check"] = "Email chưa được đăng ký";
            }

            return View();
        }
        public IActionResult Visit(string ID)
        {
            string check = ID;
            string content = "";
            if (check == null)
            {
                return Redirect("/shop");
            }
            else
            {
                ViewData["shop"] = _db.Shop.Find(check).Name;
                foreach (var item in _db.Product)
                {
                    if (item.ID_Shop.Trim().Equals(check))
                    {
                        content += $"<div class=\"col-xl-3 col-lg-4 col-md-6 wow fadeInUp\" data-wow-delay=\"0.1s\">" +
                                    $"<div class=\"product-item\">" +
                                        $"<div class=\"position-relative bg-light overflow-hidden\">" +
                                            $"<img class=\"img-fluid w-100\" src=\"{item.Image}\" alt=\"\">" +
                                        $"</div>" +
                                        $"<div class=\"text-center p-4\">" +
                                                $"<p class=\"d-block h5 mb-2\" href=\"\">{item.Name}</p>" +
                                                $"<span class=\"text-primary me-1\">{item.Price}đ</span><br>" +

                                        $"</div>" +
                                        $"<div class=\"d-flex border-top\"> " +
                                            $"<small class=\"w-100 text-center border-end py-2\">" +
                                                    $"<a class=\"text-body\" href=\"/shop/edit?ID={item.ID_Product}\">" +
                                                    $"<i class=\"fa fa-eye text-primary me-2\"></i>Edit</a></small></div></div></div>";


                    }
                }

            }
            ViewData["content"] = content;
            return View();
        }
        public IActionResult Detail(string ID)
        {
            var product = _db.Product.Find(ID);
      
            if (product != null)
            {
                ViewData["image"]=product.Image;
                ViewData["type"] = product.Type_product;
                ViewData["product-name"] = product.Name;
                ViewData["price"] = product.Price;
                ViewData["des"] = product.Description;
                ViewData["ID"] = product.ID_Product;
                var shop = _db.Shop.Find(product.ID_Shop);
                if (shop != null)
                {
                    ViewData["shop"] = $"<a href=\"/user/visit?ID={shop.ID}\">{shop.Name}</a>";
  
                }
        
            }
            else
            {
                return Redirect("/user/main");
            }
           
            return View();
        }
        public IActionResult Cart()
        {
            if (HttpContext.Session.GetString("ID")==null)
            {
                return Redirect("/user");
            }
            string content1 = "";
            string check = HttpContext.Session.GetString("ID");
            List<string> shop= new List<string>();
            foreach(var x in _db.Product_Cart)
            {
                string test =_db.Product.Find(x.ID_Product).ID_Shop;
                if(!shop.Contains(test) & test != null & x.ID_User.Trim().Equals(check))
                {

                    shop.Add(test);
                }
            }
          
            for(int i=0; i<shop.Count; i++)
            {
         
                double total = 0;
                int count = 0;
                
                content1 += $"<main>" +
                                    $"<h1 style=\"text-align: center\"  >{_db.Shop.Find(shop[i].Trim()).Name}</h1>" +
                                    $"<hr style=\"width:100%\">" +
                                    $"<div class=\"basket\">" +
                                            $"<div class=\"basket-labels\">" +
                                                $"<ul>" +
                                                        $"<li class=\"item item-heading\">Tên sản phẩm</li>\"" +
                                                        $"<li class=\"price\">Giá</li>" +
                                                        $"<li class=\"quantity\">Số lượng</li>" +
                                                        $"<li class=\"subtotal\">Tổng cộng</li>" +
                                                $"</ul>" +
                                            $"</div>";
                
            
                foreach (var x in _db.Product_Cart)
                {
                    
                    if (x.ID_User.Trim().Equals(check.Trim()) & (_db.Product.Find(x.ID_Product).ID_Shop).Equals(shop[i]))
                    {

                        var product = _db.Product.Find(x.ID_Product.Trim());
                        total += product.Price * x.Quantity;
                        count += x.Quantity;
                        content1 += $"<div class=\"basket-product\">" +
                                                $"<div class=\"item\">" +
                                                        $"<div class=\"product-image\">" +
                                                            $"<img src=\"{product.Image}\" alt=\"\" class=\"product-frame\">" +
                                                        $"</div>" +
                                                        $"<div class=\"product-details\">" +
                                                            $"<h6>{product.Name}</h6>" +
                                                        $"</div>" +
                                                 $"</div>" +
                                                 $"<div class=\"price money\">" +
                                                                $"{product.Price}" +
                                                 $"</div>" +
                                                 $"<div class=\"quantity\">" +
                                                        $"<form method=\"post\" action=\"/user/save\">" +
                                                            $"<input type=\"number\" value=\"{x.Quantity}\" min=\"1\" class=\"quantity-field\" name=\"amount\" />" +
                                                            $"<input type=\"hidden\" value=\"{x.ID_PC}\"  name=\"ID_PC1\">" +
                                                            $"<div class=\"save\">" +
                                                                $"<button type=\"submit\">Save</button>" +
                                                            $"</div>" +
                                                        $"</form>" +
                                                 $"</div>" +
                                                 $"<div class=\"subtotal money\">" +
                                                    $"{product.Price*x.Quantity}" +
                                                 $"</div>" +
                                                 $"<div class=\"remove\">" +
                                                    $"<form method=\"post\" action=\"/user/remove\">"+
                                                    $"<input type=\"hidden\" value=\"{x.ID_PC}\"  name=\"ID_PC\">" +

                                                    $"<button type=\"submit\">Remove</button>" +
                                                    $"</form>" +
                                                 $"</div>" +
                                     $"</div>";
                    
                    }
                    
                }
                content1 += "</div>";
                
                content1 += $"<aside>" +
                                $"<div class=\"summary\">" +
                                        $"<div class=\"summary-total-items\">" +
                                                $"<span class=\"total-items\"></span> Items in your Bag" +
                                        $"</div>" +
                                       
                                         $"<div class=\"summary-total\">" +
                                                $"<div class=\"total-title\">Total</div>" +
                                                $"<div class=\"money\" id=\"basket-total\">{total}</div>" +
                                         $"</div>" +
                                         $"<div class=\"summary-checkout\">" +
                                               $"<form action=\"/user/cart2\" method=\"post\">"+
                                                    $"<input type=\"hidden\" name=\"IDS\" value=\"{shop[i]}\">"+
                                                    $"<button class=\"checkout-cta\">Thanh toán</button>" +
                                                $"</form>"+
                                         $"</div>" +
                                  $"</div>" +
                              $"</aside>";
                content1 += "</main>";

            }
 
            ViewData["content1"] = content1;
            return View();
        }
        public IActionResult List_orders()
        {
            if (HttpContext.Session.GetString("ID") == null)
            {
                return Redirect("/user");
            }
            string content = "";
            string content1 = "";
            string check = HttpContext.Session.GetString("ID");
            List<string> orders = new List<string>();
            foreach (var x in _db.Product_Order)
            {
                string test = x.ID_Order;
                if (!orders.Contains(test) & test != null)
                {
                    orders.Add(test);
                }
            }
                for (int i = 0; i < orders.Count; i++)
                {
                if (check.Trim().Equals(_db.Orders.Find(orders[i]).ID_User)){
                    content1 += $"<main>" +
                                            $"<h1 style=\"text-align: center;\">{_db.Shop.Find(_db.Orders.Find(orders[i]).ID_Shop).Name}</h1>" +
                                            $"<hr style=\"width:100%\">" +
                                            $"<div class=\"basket\">" +
                                                    $"<div class=\"basket-labels\">" +
                                                        $"<ul>" +
                                                                $"<li class=\"item item-heading\">Tên sản phẩm</li>\"" +
                                                                $"<li class=\"price\">Giá</li>" +
                                                                $"<li class=\"quantity\">Số lượng</li>" +
                                                                $"<li class=\"subtotal\">Tổng cộng</li>" +
                                                        $"</ul>" +
                                                    $"</div>";
                    foreach (var x in _db.Product_Order)
                    {

                        if (_db.Orders.Find(x.ID_Order.Trim()).ID_User.Equals(check.Trim()) & x.ID_Order.Trim().Equals(orders[i].Trim()))
                        {
                            var product = _db.Product.Find(x.ID_Product.Trim());
                            content1 += $"<div class=\"basket-product\">" +
                                              $"<div class=\"item\">" +
                                                      $"<div class=\"product-image\">" +
                                                          $"<img src=\"{product.Image}\" alt=\"\" class=\"product-frame\">" +
                                                      $"</div>" +
                                                      $"<div class=\"product-details\">" +
                                                          $"<h6>{product.Name}</h6>" +
                                                      $"</div>" +
                                               $"</div>" +
                                               $"<div class=\"price money\">" +
                                                              $"{product.Price}" +
                                               $"</div>" +
                                               $"<div class=\"quantity\">" +

                                                          $"<input type=\"number\" value=\"{x.Quantity}\" min=\"1\" class=\"quantity-field\" name=\"amount\" />" +

                                               $"</div>" +
                                               $"<div class=\"subtotal money\">" +
                                                  $"{product.Price * x.Quantity}" +
                                               $"</div>" +

                                   $"</div>";

                        }


                    }

                    content1 += "</div>";
                    content1 += $"<aside>" +
                            $"<div class=\"summary\">" +
                                    $"<div class=\"summary-total-items\">" +
                                            $"<span class=\"total-items\"></span> Items in your Bag" +
                                    $"</div>" +

                                     $"<div class=\"summary-total\">" +
                                            $"<div class=\"total-title\">Total</div>" +
                                            $"<div class=\"money\" id=\"basket-total\">{_db.Orders.Find(orders[i]).Price}</div>" +
                                     $"</div>";
                    if (_db.Orders.Find(orders[i]).Status == 2)
                    {
                        content1 += $"<div class=\"summary-total\">" +
                          $"<div class=\"total-title\">Trạng thái</div>" +
                          $"<div  id=\"basket-total\">Đã giao hàng thành công</div>" +
                   $"</div>";
                    }
                    else if (_db.Orders.Find(orders[i]).Status == -1)
                    {
                        content1 += $"<div class=\"summary-total\">" +
                                $"<div class=\"total-title\">Trạng thái</div>" +
                                $"<div  id=\"basket-total\">Đã bị hủy</div>" +
                         $"</div>";
                    }
                    else if (_db.Orders.Find(orders[i]).Status == 0)
                    {
                        content1 += $"<div class=\"summary-checkout\">" +
                                    $"<form action=\"/user/cancle\" method=\"post\">" +
                                        $"<input type=\"hidden\" name=\"IDO\" value=\"{_db.Orders.Find(orders[i]).ID_Order}\">" +
                                        $"<button class=\"checkout-cta btn-danger\">Hủy đơn hàng</button>" +
                                    $"</form>" +
                                $"</div>";
                    }
                    else
                    {
                        content1 += $"<div class=\"summary-checkout\">" +
                                        $"<form action=\"/user/list_orders\" method=\"post\">" +
                                            $"<input type=\"hidden\" name=\"IDO\" value=\"{_db.Orders.Find(orders[i]).ID_Order}\">" +
                                            $"<button class=\"checkout-cta btn-success\">Xác nhận đã nhận hàng</button>" +
                                        $"</form>" +
                                    $"</div>";
                    }

                    content1 += $"</div>" +
                              $"</aside>";
                    content1 += "</main>";
                }
                       
            }
            ViewData["content1"] = content1;
            return View();
        }
        [HttpPost]
        public IActionResult List_orders(string IDO)
        {
            var order = _db.Orders.Find(IDO);
            if (order != null)
            {
                order.Status = 2;
                _db.SaveChanges();
            }
            return Redirect("/user/list_orders");
        }
        [HttpPost]
        public IActionResult cancle(string IDO)
        {
            var order = _db.Orders.Find(IDO);
            if (order != null)
            {
                order.Status = -1;
                _db.SaveChanges();
            }
            return Redirect("/user/list_orders");
        }
        [HttpPost]
        public IActionResult Cart2(string IDS)
        {
            if (HttpContext.Session.GetString("ID") == null)
            {
                return Redirect("/user");
            }
            string content = $"<input type=\"hidden\" name=\"IDS\" value=\"{IDS}\">";
            double total = 0;
            string check = HttpContext.Session.GetString("ID");
            var shop_sell = _db.Shop.Find(IDS.Trim());
            if (shop_sell != null) {
                foreach (var x in _db.Product_Cart)
                {

                    if (x.ID_User.Trim().Equals(check.Trim()) & (_db.Product.Find(x.ID_Product).ID_Shop).Trim().Equals(IDS.Trim()))
                    {

                        var product = _db.Product.Find(x.ID_Product.Trim());
        

                        content +=$"<tr>" +
                                $"<td><img src=\"{product.Image}\" class=\"img-fluid\" style=\"max-width: 100px;height:auto\"/></td>" +
                                $"<td>{product.Name}</td>" +
                                $"<td class=\"money\">{product.Price}</td>" +
                                $"<td>{x.Quantity}</td>" +
                                $"<td class=\"money\">{product.Price*x.Quantity}</td></tr>";
                      
                        total += product.Price * x.Quantity;
                    }

                }
                
            }
           
            ViewData["content"]=content;
            ViewData["tt"]=total;
            return View();
        }
        [HttpPost]
        public IActionResult Cart1(string product,string amount)
        {
            var product1 = _db.Product.Find(product.Trim());
            if (product1!=null) {
             
                foreach(var check1 in _db.Product_Cart)
                {
                    if (check1.ID_Product.Trim().Equals(product) & HttpContext.Session.GetString("ID").Equals(check1.ID_User))
                    {
                        check1.Quantity += Int32.Parse(amount);
                        _db.SaveChanges();
                        return Redirect("/user/Cart");
                    }
                }
                string idpc = "PC" + GenerateRandomNumber().ToString();
                var check_id = _db.Product_Cart.Find(idpc);//Check ID exist or not in product
                while (check_id != null)
                {
                    idpc = "PC" + GenerateRandomNumber().ToString();
                    check_id = _db.Product_Cart.Find(idpc);
                }
                string check = HttpContext.Session.GetString("ID");
                _db.Product_Cart.Add(new Product_Cart { ID_PC = idpc,ID_User=check,ID_Product=product,Quantity = Int32.Parse(amount) });
                _db.SaveChanges();
            }
            else
            {
                return Redirect("/user/Cart");

            }
            return Redirect("/user/Cart");
        }
        [HttpPost]
        public IActionResult remove(string ID_PC)
        {
            var pc = _db.Product_Cart.Find(ID_PC);
            if (pc != null)
            {
                _db.Product_Cart.Remove(pc);
                _db.SaveChanges();
            }
            return Redirect("/user/cart");
        }
        [HttpPost]
        public IActionResult buy(string address,string phone,string IDS,string total) {
            string ID_Order = "O" + GenerateRandomNumber();
            var check_IDO=_db.Orders.Find(ID_Order);
            string ID_User = HttpContext.Session.GetString("ID");
            while (check_IDO != null)
            {
                ID_Order = "O" + GenerateRandomNumber();
                check_IDO = _db.Orders.Find(ID_Order);
            }
           
            var now = DateTime.Now;
            _db.Orders.Add(new Orders { ID_Order = ID_Order, ID_User = ID_User, ID_Shop = IDS, Address_ship = address, Price = double.Parse(total), DateAt = now, Status = 0 });
            _db.SaveChanges();
            List<Product_Cart> pcl = new List<Product_Cart>();
            foreach (var x in _db.Product_Cart)
            {
                string ID_PO = "PO" + GenerateRandomNumber().ToString();
                var check_IDPO = _db.Product_Order.Find(ID_PO);
                while (check_IDPO != null)
                {
                    ID_PO = "PO" + GenerateRandomNumber();
                    check_IDPO = _db.Product_Order.Find(ID_PO);
                }
                if (x.ID_User.Trim().Equals(ID_User.Trim()) & (_db.Product.Find(x.ID_Product).ID_Shop).Trim().Equals(IDS.Trim()))
                {
                    _db.Product_Order.Add(new Product_Order { ID_PO = ID_PO, ID_Order = ID_Order, ID_Product = x.ID_Product, Quantity = x.Quantity });
                    _db.SaveChanges();
                    pcl.Add(x);
                }

            }
            foreach(var x in pcl)
            {
                _db.Product_Cart.Remove(x);
                _db.SaveChanges();
            }
            ViewData["scc"] = "Đặt hàng thành công";
            return View(); 
        }
        [HttpPost]
        public IActionResult save(string ID_PC1,int amount)
        {
            var pc = _db.Product_Cart.Find(ID_PC1);
            if (pc != null)
            {
                pc.Quantity = amount;
                _db.SaveChanges();
            }
            return Redirect("/user/cart");
        }
    }
}
