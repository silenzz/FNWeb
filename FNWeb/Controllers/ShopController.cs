using Microsoft.AspNetCore.Mvc;
using FNWeb.Models;
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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Drawing;
using Org.BouncyCastle.Crypto;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using Org.BouncyCastle.Utilities.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace FNWeb.Controllers
{

    public class ShopController : Controller
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
      
        public ShopController(DB db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            string check = HttpContext.Session.GetString("ID");
            if (check != null)
            {
                return Redirect("/shop/main");
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
                    if (temp.Role == 1)
                    {
                        ViewData["check"] = "Chưa có tài khoản này trong tài khoản shop";
                    }
                    else
                    {
                        HttpContext.Session.SetString("ID", ID);
                        HttpContext.Session.SetString("Role", "Shop");
                        return Redirect("/shop/main");
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
            string check = HttpContext.Session.GetString("ID");
            string content = "";
            if (check == null)
            {
                return Redirect("/shop");
            }
            else
            {
     
                foreach(var item in _db.Product) {
                    if (item.ID_Shop.Trim().Equals(check))
                    {
                        content += $"<div class=\"col-xl-3 col-lg-4 col-md-6 wow fadeInUp\" data-wow-delay=\"0.1s\">" +
                                    $"<div class=\"product-item\">" +
                                        $"<div class=\"position-relative bg-light overflow-hidden\">" +
                                            $"<img class=\"img-fluid w-100\" src=\"{item.Image}\" alt=\"\">" +
                                        $"</div>" +
                                        $"<div class=\"text-center p-4\">" +
                                                $"<p class=\"d-block h5 mb-2\" href=\"\">{item.Name}</p>" +
                                                $"<span class=\"text-primary me-1 money\">{item.Price}đ</span><br>" +
                                        
                                        $"</div>" +
                                        $"<div class=\"text-center p-4\">" +
                                              $"<p class=\"d-block h5 mb-2\" href=\"\">Quantity:</p>" +
                                                $"<span class=\"text-primary me-1\">{item.Quantity}</span><br>" +

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
        public IActionResult Add_product()
        {
            string check= HttpContext.Session.GetString("ID");
            if (check==null)
            {
                return Redirect("/shop");
                // Session exists and its value is stored in myValue variable
            }
            else
            {
                // Session does not exist
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add_product(IFormFile image,string name,double price,string quantity,string description,string type)
        {
            //Create ID
            string id_p= HttpContext.Session.GetString("ID")+GenerateRandomNumber().ToString();
            string ids = HttpContext.Session.GetString("ID");
            var check_id = _db.Product.Find(id_p);//Check ID exist or not in product
            while (check_id != null)
            {
                id_p = HttpContext.Session.GetString("ID") + GenerateRandomNumber().ToString();
                check_id = _db.Product.Find(id_p);
            }
            var fileName = Path.GetFileName(image.FileName);
            string fileExtension = Path.GetExtension(image.FileName);
            var saler = _db.Shop.Find(HttpContext.Session.GetString("ID"));
            if(saler != null)
            {
                //Create a path to save image
                string imgDirectory = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\img\\shop\\{saler.ID.Trim()}\\{id_p.Trim()}");
                if (!Directory.Exists(imgDirectory))
                {
                    Directory.CreateDirectory(imgDirectory);
                }

                var filePath = Path.Combine(_env.ContentRootPath, $"wwwroot\\img\\shop\\{saler.ID.Trim()}\\{id_p.Trim()}", id_p + fileExtension);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                _db.Product.Add(new Product { ID_Product = id_p, Name = name, Price = price, Quantity = Int32.Parse(quantity), Description = description,Image= $"\\img\\shop\\{saler.ID.Trim()}\\{id_p.Trim()}\\{id_p + fileExtension}",Type_product=type ,ID_Shop= ids});
                _db.SaveChanges();
                ViewData["check"] = "Thêm sản phẩm thành công";
            }
            
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string ID, string name, string email, string pass,string address)
        {
            _db.Account.Add(new Account { ID = ID, Password = pass, Role = 2 });
            _db.SaveChanges();
            _db.Shop.Add(new Shop { ID=ID,Name=name, Email=email,Address=address});
            _db.SaveChanges();
            SendEmail($"Welcome {name} with ID={ID}", $"Your account is {ID} with password={pass}\n It is very nice to work with you", email);
            ViewData["check"] = "Tạo tài khoản thành cho shop ";
            return View();
        }
        public IActionResult Edit(string ID)
        {
            string check = HttpContext.Session.GetString("ID");
            if (check == null)
            {
                return Redirect("/shop");
                // Session exists and its value is stored in myValue variable
            }
            else
            {
                var product = _db.Product.Find(ID); 
                if (product != null)
                {
                    ViewData["Name"] = product.Name;
                    ViewData["Price"] = product.Price;
                    ViewData["img"] = product.Image;
                    ViewData["Quantity"] = product.Quantity;
                    ViewData["Type"] = product.Type_product;
                    ViewData["Description"]=product.Description;
                    ViewData["ID"] = $"<input type=\"hidden\" value=\"{product.ID_Product}\" name=\"prID\" >";
                }
                // Session does not exist
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string prID, string name, double price, IFormFile image, string quantity, string type, string description)
        {
            var product_temp = _db.Product.Find(prID.Trim());
            if (product_temp != null)
            {
                product_temp.Name = name;
                product_temp.Price = price;
                product_temp.Quantity = Int32.Parse(quantity);
               
                product_temp.Type_product = type;
                product_temp.Description = description;
                _db.SaveChanges();
                if ( image==null|| image.Length == 0)
                {
                    product_temp.Image = product_temp.Image;
                    return Redirect("/shop");
                }
                else {
                    var fileName = Path.GetFileName(image.FileName);
                    if (fileName.Length == 0 || fileName == null)
                    {
         
                        return Redirect("/shop");
                    }
                    string fileExtension = Path.GetExtension(image.FileName);
                    var saler = _db.Shop.Find(HttpContext.Session.GetString("ID"));

                    if (saler != null)
                    {
                        //Create a path to save image

                        string imgDirectory = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\img\\shop\\{saler.ID.Trim()}\\{prID.Trim()}");
                        if (!Directory.Exists(imgDirectory))
                        {
                            Directory.CreateDirectory(imgDirectory);
                        }

                        var filePath = Path.Combine(_env.ContentRootPath, $"wwwroot\\img\\shop\\{saler.ID.Trim()}\\{prID.Trim()}", prID + fileExtension);
                        FileInfo file = new FileInfo(filePath);
                        file.Delete();
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }
                        product_temp.Image = $"\\img\\shop\\{saler.ID.Trim()}\\{prID.Trim()}\\{prID + fileExtension}";
                        _db.SaveChanges();
                        return Redirect("/shop");

                    }
                    else
                    {
                        return Redirect("/shop");
                    }
                }
                

            }
            return View();
        }
        public IActionResult Orders()
        {
            string check = HttpContext.Session.GetString("ID");
            string content = "";
            if (check == null)
            {
                return Redirect("/shop");
            }
            else
            {
                foreach(var item in _db.Orders)
                {
                    if (item.Status == 0) {
                        if (item.ID_Shop.Trim().Equals(check))
                        {
                            content += $"<form method=\"post\" action=\"/shop/dorders\"><tr>" +
                                    $"<td>{item.ID_Order}</td>" +
                                    $"<td>{item.ID_User} </td>" +
                                    $"<td>{item.Address_ship}</td>" +
                                    $"<td class=\"money\">{item.Price}</td>" +
                                    $"<td>{item.DateAt}</td>";


                            if (item.Status == 0)
                            {
                                content += $"<td>Đang chờ duyệt</td>";
                            }
                            else if (item.Status == 1)
                            {
                                content += $"<td  style=\"background-color:yellow\">Đang giao hàng </td>";
                            }
                            else if (item.Status == 2)
                            {
                                content += $"<td style=\"background-color:green\">Giao hàng thành công</td>";
                            }
                            else
                            {
                                content += $"<td style=\"background-color:green\">Đã hủy</td>";
                            }
                            content += $"<td><input type=\"hidden\" value={item.ID_Order} name=\"IDO\">" +
                                    $"<button class=\"btn-primary\" type=\"submit\">Xem chi tiết</button></td>";

                            content += "</form></tr>";

                        }
                    }
                       
                }
                foreach (var item in _db.Orders)
                {
                    if (item.Status == 1)
                    {
                        if (item.ID_Shop.Trim().Equals(check))
                        {
                            content += $"<form method=\"post\" action=\"/shop/dorders\"><tr>" +
                                    $"<td>{item.ID_Order}</td>" +
                                    $"<td>{item.ID_User} </td>" +
                                    $"<td>{item.Address_ship}</td>" +
                                    $"<td class=\"money\">{item.Price}</td>" +
                                    $"<td>{item.DateAt}</td>";


                            if (item.Status == 0)
                            {
                                content += $"<td>Đang chờ duyệt</td>";
                            }
                            else if (item.Status == 1)
                            {
                                content += $"<td  style=\"background-color:yellow\">Đang giao hàng </td>";
                            }
                            else if (item.Status == 2)
                            {
                                content += $"<td style=\"background-color:green\">Giao hàng thành công</td>";
                            }
                            else
                            {
                                                                content += $"<td style=\"background-color:red\">Đã hủy</td>";
                            }
                            content += $"<td><input type=\"hidden\" value={item.ID_Order} name=\"IDO\">" +
                                    $"<button class=\"btn-primary\" type=\"submit\">Xem chi tiết</button></td>";

                            content += "</form></tr>";

                        }
                    }

                }
                foreach (var item in _db.Orders)
                {
                    if (item.Status == 2)
                    {
                        if (item.ID_Shop.Trim().Equals(check))
                        {
                            content += $"<form method=\"post\" action=\"/shop/dorders\"><tr>" +
                                    $"<td>{item.ID_Order}</td>" +
                                    $"<td>{item.ID_User} </td>" +
                                    $"<td>{item.Address_ship}</td>" +
                                    $"<td class=\"money\">{item.Price}</td>" +
                                    $"<td>{item.DateAt}</td>";


                            if (item.Status == 0)
                            {
                                content += $"<td>Đang chờ duyệt</td>";
                            }
                            else if (item.Status == 1)
                            {
                                content += $"<td  style=\"background-color:yellow\">Đang giao hàng </td>";
                            }
                            else if (item.Status == 2)
                            {
                                content += $"<td style=\"background-color:green\">Giao hàng thành công</td>";
                            }
                            else
                            {
                                                                content += $"<td style=\"background-color:red\">Đã hủy</td>";
                            }
                            content += $"<td><input type=\"hidden\" value={item.ID_Order} name=\"IDO\">" +
                                    $"<button class=\"btn-primary\" type=\"submit\">Xem chi tiết</button></td>";

                            content += "</form></tr>";

                        }
                    }

                }
                foreach (var item in _db.Orders)
                {
                    if (item.Status == -1)
                    {
                        if (item.ID_Shop.Trim().Equals(check))
                        {
                            content += $"<form method=\"post\" action=\"/shop/dorders\"><tr>" +
                                    $"<td>{item.ID_Order}</td>" +
                                    $"<td>{item.ID_User} </td>" +
                                    $"<td>{item.Address_ship}</td>" +
                                    $"<td class=\"money\">{item.Price}</td>" +
                                    $"<td>{item.DateAt}</td>";


                            if (item.Status == 0)
                            {
                                content += $"<td>Đang chờ duyệt</td>";
                            }
                            else if (item.Status == 1)
                            {
                                content += $"<td  style=\"background-color:yellow\">Đang giao hàng </td>";
                            }
                            else if (item.Status == 2)
                            {
                                content += $"<td style=\"background-color:green\">Giao hàng thành công</td>";
                            }
                            else
                            {
                                                                content += $"<td style=\"background-color:red\">Đã hủy</td>";
                            }
                            content += $"<td><input type=\"hidden\" value={item.ID_Order} name=\"IDO\">" +
                                    $"<button class=\"btn-primary\" type=\"submit\">Xem chi tiết</button></td>";

                            content += "</form></tr>";

                        }
                    }

                }
            }
            ViewData["content"] = content;
            return View();
        }
        [HttpPost]
        public IActionResult Dorders(string IDO)
        {
            string check = HttpContext.Session.GetString("ID");
            var orders1 = _db.Orders.Find(IDO.Trim());
            if (HttpContext.Session.GetString("ID") == null)
            {
                return Redirect("/shop");
            }
            string content1 = "";
            string c1 = "";
            foreach (var item in _db.Orders)
            {
                if (item.ID_Shop.Trim().Equals(check) & item.ID_User.Trim().Equals(orders1.ID_User.Trim()) & item.ID_Order.Trim().Equals(orders1.ID_Order.Trim()))
                {
                    content1 += $"<tr>" +
                            $"<td>{item.ID_Order}</td>" +
                            $"<td>{item.ID_User} </td>" +
                            $"<td>{item.Address_ship}</td>" +
                            $"<td class=\"money\">{item.Price}</td>" +
                            $"<td>{item.DateAt}</td>";
                     
                    c1+=$"<tr>" +
                            $"<td>{item.ID_Order}</td>" +
                            $"<td>{item.ID_User}</td>" +
                            $"<td>{item.Address_ship}</td>" +
                            $"<td class=\"money\">{item.Price}</td>" +
                            $"<td>{item.DateAt}</td>";
                    if (item.Status == 0)
                    {
                        content1 += $"<td>Đang chờ duyệt</td>";
                        c1 += $"<td>Đang chờ duyệt</td>";
                        ViewData["ch"] = 123;
                    }
                    else if (item.Status == 1)
                    {
                        content1 += $"<td  style=\"background-color:yellow\">Đang giao hàng </td>";
                        c1 += $"<td >Đang giao hàng </td>";
                    }
                    else if (item.Status == 2)
                    {
                        content1 += $"<td>Giao hàng thành công</td>";
                        c1 += $"<td style=\"background-color:green\">Giao hàng thành công</td>";
                    }
                    else
                    {
                        content1 += $"<td>Đã hủy</td>";
                        c1 += $"<td>Đã hủy</td>";
                    }
 

                    content1 += "</tr>";
                    c1 += "</tr>";

                }
            }
            string content = $"<input type=\"hidden\" name=\"IDO\" value=\"{IDO}\">";
            string c2= $"<input type=\"hidden\" name=\"IDO\" value=\"{IDO}\">";
            double total = 0;
            foreach (var x in _db.Product_Order)
                {
                    if (x.ID_Order.Trim().Equals(IDO.Trim()))
                    {
                        var product = _db.Product.Find(x.ID_Product.Trim());
                            content += $"<tr>" +
                                $"<td><img src=\"{product.Image}\" class=\"img-fluid\" style=\"max-width: 100px;heigh:auto\"/></td>"+
                                $"<td>{product.Name}</td>" +
                                $"<td class=\"money\">{product.Price}</td>" +
                                $"<td>{x.Quantity}</td>" +
                                $"<td class=\"money\">{product.Price * x.Quantity}</td></tr>";
                    c2 += $"<tr>" +
                                $"<td><img src = \"{product.Image}\" /></td>" +
                                $"<td> {product.Name}</td>" +
                                $"<td class=\"money\">{product.Price}</td>" +
                                $"<td>{x.Quantity}</td>" +
                                $"<td class=\"money\">{product.Price * x.Quantity}</td></tr>";
                    }
                }
            ViewData["tt"] = _db.Orders.Find(IDO.Trim()).Price;
            ViewData["content1"] = content1;
            ViewData["c1"] = c1;
            ViewData["c2"] = c2;
            ViewData["content"] = content;
 
            return View();

        }
        [HttpPost]
        public IActionResult accepct(string IDO,int ac)
        {

            if (ac == 2)
            {
                var temp = _db.Orders.Find(IDO);
                if (temp != null)
                {
                    temp.Status = -1;
                    _db.SaveChanges();
                }
            }
            else
            {
                var temp=_db.Orders.Find(IDO);
                if (temp != null)
                {
                
                    foreach(var item in _db.Product_Order)
                    {
                        if (item.ID_Order.Trim().Equals(IDO.Trim())) { 
                            if (_db.Product.Find(item.ID_Product.Trim()).Quantity < item.Quantity)
                            {
                                ViewData["error"] = _db.Product.Find(item.ID_Product.Trim()).Name.ToString() + "không đủ số lượng đề nghị shop cập nhật lại số lượng để có thể đồng ý cho đơn hàng này";
                                return View();
                            }
                        }
                    }
                    foreach (var item in _db.Product_Order)
                    {
                        if (item.ID_Order.Trim().Equals(IDO.Trim()))
                        {
                            var product = _db.Product.Find(item.ID_Product.Trim());
                            if (product != null)
                            {
                                product.Quantity = product.Quantity - item.Quantity;
                                _db.SaveChanges();
                            }
                      
                        }
                    }
                    temp.Status = 1;
                    _db.SaveChanges();
                    ViewData["done"] = "Thành công";
    
                }
               
            }
            return Redirect("/shop/orders");
        }
    }
}
