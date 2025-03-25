using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using DACS_WebClothes.Models;
using DACS_WebClothes.ViewModels;
using DACS_WebClothes.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Net;

namespace DACS_WebClothes.Controllers
{
    public class CartController : Controller
    {
        private readonly WebClothesContext _context;
        private readonly IVnPayService _vnPayservice;
        private const string CartSession = "CartSession";
        public CartController(WebClothesContext context, IVnPayService vnpayService)
        {
            _context = context;
            _vnPayservice = vnpayService;
        }
        [HttpPost]
        public IActionResult AddItem(int ProductId, int Quantity, string size)
        {
            Quantity = 1;
            var product = _context.Products.Find(ProductId);
            var cart = HttpContext.Session.GetString(CartSession);
            if (!string.IsNullOrEmpty(cart))
            {
                var list = JsonConvert.DeserializeObject<List<CartItem>>(cart);
                if (list.Exists(x => x.Product.ProductId == ProductId && x.Size == size))
                {
                    foreach (var item in list)
                    {
                        if (item.Product.ProductId == ProductId && item.Size == size)
                        {
                            item.Quantity += Quantity;
                        }
                    }
                }
                else
                {
                    var item = new CartItem();
                    item.Product = product;
                    item.Quantity = Quantity;
                    item.Size = size; // Cập nhật thông tin về kích thước
                    list.Add(item);
                }
                HttpContext.Session.SetString(CartSession, JsonConvert.SerializeObject(list));
            }
            else
            {
                var item = new CartItem();
                item.Product = product;
                item.Quantity = Quantity;
                item.Size = size; // Cập nhật thông tin về kích thước
                var list = new List<CartItem>();
                list.Add(item);
                HttpContext.Session.SetString(CartSession, JsonConvert.SerializeObject(list));
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Index()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();
            var cart = HttpContext.Session.GetString(CartSession);
            var list = new List<CartItem>();
            if (!string.IsNullOrEmpty(cart))
            {
                list = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            }
            var cartViewModel = new CartViewModel
            {
                Menus = menus,
                CartItems = list
            };
            return View(cartViewModel);
        }
        public IActionResult DeleteAll()
        {
            HttpContext.Session.Remove(CartSession);
            return Json(new
            {
                status = true
            });
        }
        public IActionResult Delete(int id)
        {
            var sessionCart = JsonConvert.DeserializeObject<List<CartItem>>(HttpContext.Session.GetString(CartSession));
            sessionCart.RemoveAll(x => x.Product.ProductId == id);
            HttpContext.Session.SetString(CartSession, JsonConvert.SerializeObject(sessionCart));
            return Json(new
            {
                status = true
            });
        }

        public IActionResult Update(string cartModel)
        {
            var jsonCart = JsonConvert.DeserializeObject<List<CartItem>>(cartModel);
            var sessionCart = JsonConvert.DeserializeObject<List<CartItem>>(HttpContext.Session.GetString(CartSession));
            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.ProductId == item.Product.ProductId);
                if (jsonItem != null)
                {
                    item.Quantity = jsonItem.Quantity;
                }
            }
            HttpContext.Session.SetString(CartSession, JsonConvert.SerializeObject(sessionCart));
            return Json(new
            {
                status = true
            });
        }
        [HttpGet]
        public async Task<IActionResult> Order(string name)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();
            var users = new User();
            if (User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                if (username != null)
                {
                    users = await _context.Users.FirstOrDefaultAsync(m => m.FullName == username);
                }
            }
            var cart = HttpContext.Session.GetString(CartSession);
            var list = new List<CartItem>();
            if (!string.IsNullOrEmpty(cart))
            {
                list = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            }
            var cartViewModel = new CartViewModel
            {
                Menus = menus,
                CartItems = list,
                Register = users
            };
            return View(cartViewModel);
        }
        public static string GenerateOrderId()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] orderIdChars = new char[9];
            for (int i = 0; i < orderIdChars.Length; i++)
            {
                orderIdChars[i] = chars[random.Next(chars.Length)];
            }
            string orderId = new string(orderIdChars);
            return orderId;
        }
        [HttpPost]
        public async Task<IActionResult> Order(Decimal Amount, string inputDcName, string paymentMethod, string transportMethod, string payment = "COD")
        {
            var order = new Cart();
            var pay = new Payment();
            order.Hidden = 0;
            order.DateBegin = DateTime.Now;
            order.OrderId = GenerateOrderId();
            order.Status = "Chờ Xác Nhận";
            var users = new User();
            string username = User.Identity.Name;
            if (username != null)
            users = await _context.Users.FirstOrDefaultAsync(m => m.FullName == username);
            order.UserId = users.UserId;
            if (payment == "Hoàn tất đơn hàng (VNPay)")
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = Convert.ToDouble(Amount),
                    CreatedDate = DateTime.Now,
                    Description = "Thanh toán đơn hàng",
                    FullName = users.FullName,
                    OrderID = _context.Payments.Count() + 1 
                };
                _context.Carts.Add(order);
                _context.SaveChanges();
                var id = order.CartId;
                var cart = JsonConvert.DeserializeObject<List<CartItem>>(HttpContext.Session.GetString(CartSession));
                foreach (var item in cart)
                {
                    var detail = new CartDetail();
                    detail.ProductId = item.Product.ProductId;
                    detail.CartId = id;
                    detail.SoldNum = item.Quantity;
                    _context.CartDetails.Add(detail);
                    _context.SaveChanges();
                }

                var DsName = _context.Discounts.SingleOrDefault(p => p.DiscountName == inputDcName);
                if (DsName != null)
                {
                    pay.CartId = id;
                    pay.PaymentDate = order.DateBegin;
                    pay.UserId = order.UserId;
                    pay.PaymentMethod = paymentMethod;
                    pay.TransportMethod = transportMethod;
                    pay.Amount = Amount;
                    pay.DiscountId = DsName != null ? DsName.DiscountId : null;

                    _context.Payments.Add(pay);
                    _context.SaveChanges();
                }
                else
                {
                    pay.CartId = id;
                    pay.PaymentDate = order.DateBegin;
                    pay.UserId = order.UserId;
                    pay.PaymentMethod = paymentMethod;
                    pay.TransportMethod = transportMethod;
                    pay.Amount = Amount;

                    _context.Payments.Add(pay);
                    _context.SaveChanges();
                }
                return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
            }
            try
            {
                _context.Carts.Add(order);
                _context.SaveChanges();
                var id = order.CartId;
                var cart = JsonConvert.DeserializeObject<List<CartItem>>(HttpContext.Session.GetString(CartSession));
                foreach (var item in cart)
                {
                    var detail = new CartDetail();
                    detail.ProductId = item.Product.ProductId;
                    detail.CartId = id;
                    detail.SoldNum = item.Quantity;
                    _context.CartDetails.Add(detail);
                    _context.SaveChanges();
                }

                var DsName = _context.Discounts.SingleOrDefault(p => p.DiscountName == inputDcName);
                if (DsName != null)
                {
                    pay.CartId = id;
                    pay.PaymentDate = order.DateBegin;
                    pay.UserId = order.UserId;
                    pay.PaymentMethod = paymentMethod;
                    pay.TransportMethod = transportMethod;
                    pay.Amount = Amount;
                    pay.DiscountId = DsName != null ? DsName.DiscountId : null;

                    _context.Payments.Add(pay);
                    _context.SaveChanges();
                }
                else
                {
                    pay.CartId = id;
                    pay.PaymentDate = order.DateBegin;
                    pay.UserId = order.UserId;
                    pay.PaymentMethod = paymentMethod;
                    pay.TransportMethod = transportMethod;
                    pay.Amount = Amount;

                    _context.Payments.Add(pay);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Redirect("/hoan-thanh");
        }
        private async Task SendOrderConfirmationEmail(string recipientEmail, int orderId)
        {
            try
            {
                var fromAddress = new MailAddress("pductai14@gmail.com", "Phạm Đức Tài");
                var toAddress = new MailAddress(recipientEmail);
                const string fromPassword = "01289551089";
                const string subject = "Confirmation of Your Order";
                string body = $"Your order with ID {orderId} has been successfully placed.";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 5217,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    await smtp.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ khi gửi email không thành công
                throw;
            }
        }
        public async Task<IActionResult> Success()
        {
            DeleteAll();
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.FullName == User.Identity.Name);
            if (currentUser == null)
            {
                return View("NotFound");
            }
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var cartViewModel = new CartViewModel
            {
                Menus = menus,
                Register = currentUser
            };
            return View(cartViewModel);
        }
        public IActionResult GetCartItemCount()
        {
            var cart = HttpContext.Session.GetString("CartSession");
            int itemCount = 0;
            if (!string.IsNullOrEmpty(cart))
            {
                var list = JsonConvert.DeserializeObject<List<CartItem>>(cart);
                itemCount = list.Sum(x => x.Quantity);
            }
            return Json(itemCount);
        }
        [HttpGet]
        public JsonResult ApplyPromoCode(string code, decimal? totalPrice)
        {
            if (String.IsNullOrEmpty(code))
            {
                return Json(new { status = false });
            }

            decimal? finalPrice = totalPrice ?? 0;
            decimal? discounted = 0;

            var promo = _context.Discounts.SingleOrDefault(p => p.DiscountName == code);

            if (promo != null)
            {
                if (promo.ExpirationDate < DateTime.Now)
                {
                    return Json(new { status = false, error = "Mã khuyến mãi đã hết hạn" });
                }

                if (promo.DiscountNum != null)
                {
                    discounted = promo.DiscountNum;
                    if (finalPrice != null)
                    {
                        finalPrice -= promo.DiscountNum;
                        if (finalPrice < 0)
                        {
                            finalPrice = 0;
                        }
                    }
                }
                else if (promo.DiscountPercent != null)
                {
                    discounted = promo.DiscountPercent * finalPrice;
                    if (finalPrice != null)
                    {
                        finalPrice -= finalPrice * promo.DiscountPercent;
                    }
                }
            }
            return Json(new { status = true, finalPrice = finalPrice, discounted = discounted });
        }
        public async Task<IActionResult> PaymentFail()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var cartViewModel = new CartViewModel
            {
                Menus = menus,
            };
            return View(cartViewModel);
        }

        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            //neu giao dich khong thanh cong
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay";
                return Redirect("order");
            }
            //Luu dơn hàng

            return Redirect("/hoan-thanh");
        }
        [Authorize]
        public async Task<IActionResult> UserOrders()
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.FullName == User.Identity.Name);
            if (currentUser == null)
            {
                return View("NotFound");
            }
            var userOrders = await _context.Carts
                .Where(c => c.UserId == currentUser.UserId)
                .OrderByDescending(c => c.DateBegin)
                .ToListAsync();
            var cartViewModel = new CartViewModel
            {
                Menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync(),
                UserOrders = userOrders
            };
            return View(cartViewModel);
        }
        public async Task<IActionResult> OrderDetails(int cartId)
        {
            var users = new User();
            string username = User.Identity.Name;
            if (username != null)
                users = await _context.Users.FirstOrDefaultAsync(m => m.FullName == username);

            // Lấy thông tin đơn hàng từ database
            var order = await _context.Carts
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.UserId == users.UserId);

            if (order == null)
            {
                return View("NotFound");
            }

            // Truy vấn thông tin thanh toán từ bảng Payments
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.CartId == order.CartId);

            if (payment == null)
            {
                return View("NotFound");
            }

            // Lấy thông tin thanh toán từ đối tượng Payment
            var paymentMethod = payment.PaymentMethod;
            var transportMethod = payment.TransportMethod;
            var amount = payment.Amount;

            var orderViewModel = new CartViewModel
            {
                Menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync(),
                Order = order,
                PaymentMethod = paymentMethod,
                TransportMethod = transportMethod,
                Amount = amount.GetValueOrDefault()
            };

            return View(orderViewModel);
        }
    }
}
