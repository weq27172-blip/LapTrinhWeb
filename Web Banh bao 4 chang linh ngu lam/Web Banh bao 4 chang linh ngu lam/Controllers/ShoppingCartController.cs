using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Banh_bao_4_chang_linh_ngu_lam.Models;

namespace Web_Banh_bao_4_chang_linh_ngu_lam.Controllers
{
    public class ShoppingCartController : Controller
    {
        private DBBanhBaoTuanDatEntities db = new DBBanhBaoTuanDatEntities();

        // GET CART
        private List<CartItem> GetCart()
        {
            if (Session["Cart"] == null)
                Session["Cart"] = new List<CartItem>();
            return Session["Cart"] as List<CartItem>;
        }

        // ADD TO CART
        public ActionResult AddToCart(int id)
        {
            var product = db.Products.Find(id);
            if (product == null) return RedirectToAction("Index", "Product");

            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.Product.ProductID == id);

            if (item != null)
                item.Quantity++;
            else
                cart.Add(new CartItem { Product = product, Quantity = 1, FreeQuantity = 0 });

            return RedirectToAction("ShowCart");
        }

        // SHOW CART
        public ActionResult ShowCart() => View(GetCart());

        // UPDATE CART
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCart(int[] productIds, int[] quantities)
        {
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

            return RedirectToAction("ShowCart");
        }

        // REMOVE ITEM
        public ActionResult RemoveItem(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.Product.ProductID == id);
            if (item != null)
                cart.Remove(item);
            return RedirectToAction("ShowCart");
        }

        // CLEAR CART
        public ActionResult ClearCart()
        {
            Session["Cart"] = null;
            return RedirectToAction("ShowCart");
        }

        // CHECKOUT GET
        public ActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["Message"] = "Giỏ hàng trống.";
                return RedirectToAction("ShowCart");
            }

            ViewBag.DiscountCodes = db.MaGiamGias
                                      .Where(d => d.IsActive && (d.ExpireDate == null || d.ExpireDate >= DateTime.Now))
                                      .ToList();

            return View(cart);
        }

        // CHECKOUT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(
            string NameCus, string PhoneCus, string EmailCus, string AddressCus,
            string PaymentType, string CardHolder, string CardNumber, string ExpiryDate, string CVV,
            bool SaveCard = false, string DiscountCode = null)
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["Message"] = "Giỏ hàng trống.";
                return RedirectToAction("ShowCart");
            }

            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    // CUSTOMER
                    var customer = db.Customers.FirstOrDefault(c => c.EmailCus == EmailCus && c.PhoneCus == PhoneCus);
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

                    // SAVE CARD OPTION
                    if (PaymentType == "Card" && SaveCard)
                    {
                        customer.SavedPaymentMethodType = "Card";
                        customer.SavedCardLast4 = CardNumber.Substring(CardNumber.Length - 4);
                        customer.SavePaymentMethod = true;
                        db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    // CALCULATE TOTAL
                    decimal totalAmount = cart.Sum(x => x.TotalPrice);

                    // APPLY DISCOUNT
                    MaGiamGia discount = null;
                    if (!string.IsNullOrEmpty(DiscountCode))
                    {
                        discount = db.MaGiamGias.FirstOrDefault(d =>
                            d.Code == DiscountCode &&
                            d.IsActive &&
                            (d.ExpireDate == null || d.ExpireDate >= DateTime.Now));

                        if (discount != null)
                        {
                            // BUY X GET Y
                            if (discount.Type == 1)
                            {
                                int x = discount.BuyQuantity ?? 0;
                                int y = discount.FreeQuantity ?? 0;

                                foreach (var item in cart)
                                {
                                    item.FreeQuantity = 0;

                                    if (x > 0 && y > 0 && item.Quantity >= x)
                                    {
                                        int cycles = item.Quantity / x;
                                        int freeItems = cycles * y;
                                        item.FreeQuantity = freeItems;
                                    }
                                }
                            }
                            // PERCENT OFF
                            else if (discount.Type == 2)
                            {
                                totalAmount -= totalAmount * (discount.PercentOff.GetValueOrDefault() / 100);
                            }
                        }
                    }

                    // CREATE ORDER
                    var order = new OrderPro
                    {
                        DateOrder = DateTime.Now,
                        IDCus = customer.IDCus,
                        DeliveryAddress = AddressCus,
                        PaymentMethod = PaymentType == "Card" ? "Thẻ ngân hàng" : "Thanh toán khi nhận hàng",
                        TotalAmount = totalAmount,
                        Status = "Chờ xác nhận"
                    };
                    db.OrderProes.Add(order);
                    db.SaveChanges();

                    // SAVE ORDER DETAILS
                    foreach (var item in cart)
                    {
                        // Paid items
                        db.OrderDetails.Add(new OrderDetail
                        {
                            IDOrder = order.ID,
                            IDProduct = item.Product.ProductID,
                            Quantity = item.Quantity,
                            UnitPrice = item.Product.Price
                        });

                        // Free items (price = 0)
                        if (item.FreeQuantity > 0)
                        {
                            db.OrderDetails.Add(new OrderDetail
                            {
                                IDOrder = order.ID,
                                IDProduct = item.Product.ProductID,
                                Quantity = item.FreeQuantity,
                                UnitPrice = 0
                            });
                        }
                    }

                    db.SaveChanges();
                    tran.Commit();

                    Session["Cart"] = null;

                    return RedirectToAction("OrderSuccess", new { id = order.ID });
                }
                catch
                {
                    tran.Rollback();
                    TempData["Error"] = "Lỗi xảy ra khi thanh toán.";
                    return RedirectToAction("Checkout");
                }
            }
        }

        // ORDER SUCCESS
        public ActionResult OrderSuccess(int id)
        {
            var order = db.OrderProes.Find(id);
            if (order == null) return RedirectToAction("Index", "Home");
            return View(order);
        }
    }
}