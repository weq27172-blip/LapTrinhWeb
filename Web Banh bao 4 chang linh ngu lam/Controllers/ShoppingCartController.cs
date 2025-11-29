using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Web_Banh_bao_4_chang_linh_ngu_lam.Models;

namespace Web_Banh_bao_4_chang_linh_ngu_lam.Controllers
{
    public class ShoppingCartController : Controller
    {
        private DBBanhBaoTuanDatEntities db = new DBBanhBaoTuanDatEntities();

        // ===== HELPER: GET CART =====
        private List<CartItem> GetCart()
        {
            if (Session["Cart"] == null)
                Session["Cart"] = new List<CartItem>();
            return Session["Cart"] as List<CartItem>;
        }

        // ===== HELPER: CALCULATE SHIPPING FEE =====
        private decimal GetShippingFee(string province)
        {
            var shippingFees = new Dictionary<string, decimal>
            {
                {"TPHCM", 0},
                {"Bình Dương", 15000},
                {"Đồng Nai", 20000},
                {"Long An", 25000},
                {"Bà Rịa - Vũng Tàu", 30000},
                {"Tây Ninh", 35000},
                {"Bình Phước", 40000},
                {"Cần Thơ", 45000},
                {"An Giang", 50000},
                {"Đồng Tháp", 45000},
                {"Tiền Giang", 30000},
                {"Vĩnh Long", 40000},
                {"Bến Tre", 35000},
                {"Trà Vinh", 40000},
                {"Sóc Trăng", 45000},
                {"Hậu Giang", 45000},
                {"Kiên Giang", 50000},
                {"Bạc Liêu", 50000},
                {"Cà Mau", 55000},
                {"Bình Thuận", 40000}
            };

            return string.IsNullOrEmpty(province) || !shippingFees.ContainsKey(province)
                ? 0
                : shippingFees[province];
        }

        // ===== ADD TO CART =====
        public ActionResult AddToCart(int id)
        {
            var product = db.Products.Find(id);
            if (product == null || product.IsDeleted == true)
            {
                TempData["Error"] = "Sản phẩm không tồn tại hoặc đã bị xóa.";
                return RedirectToAction("Index", "Product");
            }

            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.Product.ProductID == id);

            if (item != null)
                item.Quantity++;
            else
                cart.Add(new CartItem { Product = product, Quantity = 1 });

            TempData["Success"] = $"Đã thêm '{product.NamePro}' vào giỏ hàng!";
            return RedirectToAction("ShowCart");
        }

        // ===== SHOW CART =====
        public ActionResult ShowCart()
        {
            return View(GetCart());
        }

        // ===== UPDATE CART =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCart(int[] productIds, int[] quantities)
        {
            if (productIds == null || quantities == null || productIds.Length != quantities.Length)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("ShowCart");
            }

            var cart = GetCart();
            for (int i = 0; i < productIds.Length; i++)
            {
                var id = productIds[i];
                var qty = quantities[i];

                var item = cart.FirstOrDefault(x => x.Product.ProductID == id);
                if (item != null)
                {
                    if (qty <= 0)
                        cart.Remove(item);
                    else
                        item.Quantity = qty;
                }
            }

            TempData["Success"] = "Giỏ hàng đã được cập nhật!";
            return RedirectToAction("ShowCart");
        }

        // ===== REMOVE ITEM =====
        public ActionResult RemoveItem(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.Product.ProductID == id);
            if (item != null)
            {
                cart.Remove(item);
                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
            }
            return RedirectToAction("ShowCart");
        }

        // ===== CLEAR CART =====
        public ActionResult ClearCart()
        {
            Session["Cart"] = null;
            TempData["Success"] = "Đã xóa toàn bộ giỏ hàng!";
            return RedirectToAction("ShowCart");
        }

        // ===== CHECKOUT GET =====
        public ActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["Message"] = "Giỏ hàng trống.";
                return RedirectToAction("ShowCart");
            }

            return View(cart);
        }

        // ===== CHECKOUT POST =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(
            string NameCus,
            string PhoneCus,
            string EmailCus,
            string AddressCus,
            string Province,
            string PaymentType,
            string CardHolder,
            string CardNumber,
            string ExpiryDate,
            string CVV,
            string DiscountCode)
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("ShowCart");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(NameCus) ||
                string.IsNullOrWhiteSpace(PhoneCus) ||
                string.IsNullOrWhiteSpace(AddressCus) ||
                string.IsNullOrWhiteSpace(Province))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin!";
                return View(cart);
            }

            // Validate card payment
            if (PaymentType == "Card")
            {
                if (string.IsNullOrWhiteSpace(CardHolder) ||
                    string.IsNullOrWhiteSpace(CardNumber) ||
                    string.IsNullOrWhiteSpace(ExpiryDate) ||
                    string.IsNullOrWhiteSpace(CVV))
                {
                    TempData["Error"] = "Vui lòng điền đầy đủ thông tin thẻ!";
                    return View(cart);
                }
            }

            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    // ===== FIND OR CREATE CUSTOMER =====
                    var customer = db.Customers.FirstOrDefault(c => c.PhoneCus == PhoneCus);
                    if (customer == null)
                    {
                        customer = new Customer
                        {
                            NameCus = NameCus,
                            PhoneCus = PhoneCus,
                            EmailCus = EmailCus,
                            AddressCus = AddressCus
                        };
                        db.Customers.Add(customer);
                        db.SaveChanges();
                    }

                    // ===== CALCULATE SUBTOTAL =====
                    decimal subtotal = cart.Sum(x => x.Product.Price * x.Quantity);

                    // ===== CALCULATE SHIPPING FEE =====
                    decimal shippingFee = GetShippingFee(Province);

                    // ===== APPLY DISCOUNT =====
                    MaGiamGia appliedDiscount = null;
                    int? discountID = null;
                    List<OrderDetail> freeItems = new List<OrderDetail>();

                    if (!string.IsNullOrWhiteSpace(DiscountCode))
                    {
                        appliedDiscount = db.MaGiamGias.FirstOrDefault(d =>
                            d.Code == DiscountCode &&
                            d.IsActive == true &&
                            (d.ExpireDate == null || d.ExpireDate >= DateTime.Now));

                        if (appliedDiscount != null)
                        {
                            discountID = appliedDiscount.ID;

                            // TYPE 1: BUY X GET Y
                            if (appliedDiscount.Type == 1 &&
                                appliedDiscount.BuyQuantity.HasValue &&
                                appliedDiscount.FreeQuantity.HasValue)
                            {
                                int buyQty = appliedDiscount.BuyQuantity.Value;
                                int freeQty = appliedDiscount.FreeQuantity.Value;
                                int totalItems = cart.Sum(x => x.Quantity);

                                if (totalItems >= buyQty)
                                {
                                    int setsEarned = totalItems / buyQty;
                                    int totalFreeItems = setsEarned * freeQty;

                                    // Use first product as free gift
                                    var freeProduct = cart.First().Product;
                                    freeItems.Add(new OrderDetail
                                    {
                                        IDProduct = freeProduct.ProductID,
                                        Quantity = totalFreeItems,
                                        UnitPrice = 0 // FREE
                                    });
                                }
                            }
                            // TYPE 2: PERCENT OFF
                            else if (appliedDiscount.Type == 2 && appliedDiscount.PercentOff.HasValue)
                            {
                                decimal discountPercent = appliedDiscount.PercentOff.Value;
                                decimal discountAmount = subtotal * (discountPercent / 100);
                                subtotal -= discountAmount;
                            }
                        }
                    }

                    // ===== CALCULATE TOTAL AMOUNT =====
                    decimal totalAmount = subtotal + shippingFee;

                    // ===== CREATE ORDER =====
                    var order = new OrderPro
                    {
                        DateOrder = DateTime.Now,
                        IDCus = customer.IDCus,
                        DeliveryAddress = AddressCus,
                        Province = Province,
                        PaymentMethod = PaymentType == "Card" ? "Thẻ ngân hàng" :
                                       PaymentType == "BankTransfer" ? "Chuyển khoản" :
                                       "Thanh toán khi nhận hàng",
                        ShippingFee = shippingFee,
                        TotalAmount = totalAmount,
                        Status = "Chờ xác nhận",
                        DiscountID = discountID
                    };

                    db.OrderProes.Add(order);
                    db.SaveChanges();

                    // ===== CREATE ORDER DETAILS (PAID ITEMS) =====
                    foreach (var item in cart)
                    {
                        var detail = new OrderDetail
                        {
                            IDOrder = order.ID,
                            IDProduct = item.Product.ProductID,
                            Quantity = item.Quantity,
                            UnitPrice = item.Product.Price
                        };
                        db.OrderDetails.Add(detail);
                    }

                    // ===== ADD FREE ITEMS =====
                    foreach (var freeItem in freeItems)
                    {
                        freeItem.IDOrder = order.ID;
                        db.OrderDetails.Add(freeItem);
                    }

                    db.SaveChanges();
                    tran.Commit();

                    // ===== CLEAR CART =====
                    Session["Cart"] = null;

                    return RedirectToAction("OrderSuccess", new { id = order.ID });
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    TempData["Error"] = "Lỗi xảy ra khi thanh toán: " + ex.Message;
                    return View(cart);
                }
            }
        }

        // ===== ORDER SUCCESS =====
        public ActionResult OrderSuccess(int id)
        {
            var order = db.OrderProes
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .Include(o => o.MaGiamGia)
                .FirstOrDefault(o => o.ID == id);

            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }

        // ===== VERIFY DISCOUNT (AJAX) =====
        [HttpGet]
        public JsonResult VerifyDiscount(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Json(new { success = false, message = "Mã không hợp lệ" }, JsonRequestBehavior.AllowGet);
            }

            var discount = db.MaGiamGias.FirstOrDefault(d =>
                d.Code == code &&
                d.IsActive == true &&
                (d.ExpireDate == null || d.ExpireDate >= DateTime.Now));

            if (discount == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không tồn tại hoặc đã hết hạn" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = true,
                discount = new
                {
                    discount.Type,
                    discount.PercentOff,
                    discount.BuyQuantity,
                    discount.FreeQuantity
                }
            }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}