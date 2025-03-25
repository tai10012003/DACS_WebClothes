using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using DACS_WebClothes.Models;
using DACS_WebClothes.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;

namespace DACS_WebClothes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminPermission")]
    public class AdminOrderController : Controller
    {
        private readonly WebClothesContext _context;

        public AdminOrderController(WebClothesContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string SearchString = "")
        {
            IQueryable<Cart> query = _context.Carts
                .Include(cart => cart.User)
                .Include(cart => cart.CartDetails);

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(x => x.OrderId.ToUpper().Contains(SearchString.ToUpper()));
            }

            var orders = await query
                .OrderByDescending(cart => cart.DateBegin)
                .ToListAsync();

            return View(orders);
        }


        [HttpGet]
        public async Task<IActionResult> EditAndDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .Include(c => c.User)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.CartId == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.CartDetails = order.CartDetails;
            ViewBag.User = order.User;
            ViewBag.Payments = order.Payments;

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAndDetails(int id, Cart cart)
        {
            ModelState.Remove("User");
            ModelState.Remove("Payments");
            if (id != cart.CartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCart = await _context.Carts.FindAsync(id);
                    if (existingCart == null)
                    {
                        return NotFound();
                    }
                    existingCart.Status = cart.Status;

                    _context.Carts.Update(existingCart);
                    await _context.SaveChangesAsync();

                    return Redirect("~/Admin/AdminOrder/Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewBag.CartDetails = cart.CartDetails;
            ViewBag.User = cart.User;
            ViewBag.Payments = cart.Payments;
            return View(cart);
        }


        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.CartId == id);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.CartId == id);

            if (cart == null)
            {
                return NotFound();
            }
            ViewBag.CartDetails = cart.CartDetails;
            ViewBag.User = cart.User;
            ViewBag.Payments = cart.Payments;
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.CartId == id);

            if (cart != null)
            {
                _context.CartDetails.RemoveRange(cart.CartDetails);
                _context.Carts.Remove(cart);

                await _context.SaveChangesAsync();
            }

            return Redirect("~/Admin/AdminOrder/Index");
        }
        public async Task<IActionResult> ExportToPdf(int id)
        {
            var order = await _context.Carts
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Product)
                .Include(c => c.User)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.CartId == id);

            if (order == null)
            {
                return NotFound();
            }

            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    var writer = new PdfWriter(memoryStream);
                    var pdf = new PdfDocument(writer);
                    var document = new iText.Layout.Document(pdf);

                    var defaultFontPath = "C:/Windows/Fonts/arial.ttf";
                    var defaultFont = PdfFontFactory.CreateFont(defaultFontPath);

                    document.Add(new Paragraph("Cửa hàng: TripleT Store").SetFont(defaultFont).SetFontSize(12));
                    document.Add(new Paragraph("Địa chỉ: 42 Tôn Thất Thiệp, Bến Nghé, Quận 1, Thành phố Hồ Chí Minh").SetFont(defaultFont));
                    document.Add(new Paragraph("Điện thoại: 0927749820").SetFont(defaultFont).SetFontSize(12));
                    document.Add(new Paragraph("Email: tripletshop@gmail.com").SetFont(defaultFont).SetFontSize(12));

                    for (int i = 0; i < 3; i++)
                    {
                        document.Add(new Paragraph(" ").SetFont(defaultFont).SetFontSize(12));
                    }

                    document.Add(new Paragraph("HÓA ĐƠN BÁN HÀNG").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20).SetFont(defaultFont));
                    document.Add(new Paragraph($"Mã đơn hàng: {order.OrderId}").SetFont(defaultFont).SetFontSize(12));
                    document.Add(new Paragraph($"Ngày đặt hàng: {order.DateBegin.ToString("dd/MM/yyyy HH:mm:ss")}").SetFont(defaultFont).SetFontSize(12));
                    document.Add(new Paragraph($"Tên khách hàng: {order.User.FullName}").SetFont(defaultFont).SetFontSize(12));
                    document.Add(new Paragraph($"Email: {order.User.Email}").SetFont(defaultFont).SetFontSize(12));
                    document.Add(new Paragraph($"Số điện thoại: {order.User.Phone}").SetFont(defaultFont).SetFontSize(12));
                    document.Add(new Paragraph($"Địa chỉ: {order.User.Address}").SetFont(defaultFont).SetFontSize(12));
                    document.Add(new Paragraph($"Phương thức vận chuyển: {order.Payments.TransportMethod}").SetFont(defaultFont).SetFontSize(12));
                    document.Add(new Paragraph($"Phương thức thanh toán: {order.Payments.PaymentMethod}").SetFont(defaultFont).SetFontSize(12));
                    for (int i = 0; i < 2; i++)
                    {
                        document.Add(new Paragraph(" ").SetFont(defaultFont).SetFontSize(12));
                    }
                    var table = new Table(new float[] { 1, 3, 2, 2, 2 });
                    table.SetWidth(UnitValue.CreatePercentValue(100));

                    table.AddHeaderCell(new Paragraph("STT").SetFont(defaultFont).SetTextAlignment(TextAlignment.CENTER));
                    table.AddHeaderCell(new Paragraph("Tên sản phẩm").SetFont(defaultFont).SetTextAlignment(TextAlignment.CENTER));
                    table.AddHeaderCell(new Paragraph("Số lượng").SetFont(defaultFont).SetTextAlignment(TextAlignment.CENTER));
                    table.AddHeaderCell(new Paragraph("Giá").SetFont(defaultFont).SetTextAlignment(TextAlignment.CENTER));
                    table.AddHeaderCell(new Paragraph("Thành tiền").SetFont(defaultFont).SetTextAlignment(TextAlignment.CENTER));

                    int index = 1;
                    foreach (var detail in order.CartDetails)
                    {
                        table.AddCell(new Cell().Add(new Paragraph(index.ToString()).SetFont(defaultFont).SetTextAlignment(TextAlignment.CENTER)));
                        table.AddCell(new Cell().Add(new Paragraph(detail.Product.Name).SetFont(defaultFont).SetTextAlignment(TextAlignment.CENTER)));
                        table.AddCell(new Cell().Add(new Paragraph(detail.SoldNum.ToString()).SetFont(defaultFont).SetTextAlignment(TextAlignment.CENTER)));
                        table.AddCell(new Cell().Add(new Paragraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:c}", detail.Product.Price ?? 0)).SetFont(defaultFont).SetTextAlignment(TextAlignment.CENTER)));
                        decimal itemTotalPrice = (detail.Product.Price ?? 0) * detail.SoldNum;
                        table.AddCell(new Cell().Add(new Paragraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:c}", itemTotalPrice)).SetFont(defaultFont).SetTextAlignment(TextAlignment.CENTER)));
                        index++;
                    }
                    document.Add(table);
                    for (int i = 0; i < 1; i++)
                    {
                        document.Add(new Paragraph(" ").SetFont(defaultFont).SetFontSize(12));
                    }
                    document.Add(new Paragraph($"Tổng tiền: {String.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:c}", order.Payments.Amount)}").SetTextAlignment(TextAlignment.RIGHT).SetFontSize(14).SetBold().SetFont(defaultFont));
                    for (int i = 0; i < 1; i++)
                    {
                        document.Add(new Paragraph(" ").SetFont(defaultFont).SetFontSize(12));
                    }
                    document.Add(new Paragraph("Người mua hàng").SetTextAlignment(TextAlignment.RIGHT).SetFontSize(12).SetFont(defaultFont));
                    document.Add(new Paragraph("(Ký, ghi rõ họ tên)").SetTextAlignment(TextAlignment.RIGHT).SetFontSize(12).SetFont(defaultFont));
                   
                    document.Close();
                    return File(memoryStream.ToArray(), "application/pdf", $"HD_{order.OrderId}.pdf");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return StatusCode(500, "An error occurred while generating the PDF.");
                }
            }
        }
    }
}