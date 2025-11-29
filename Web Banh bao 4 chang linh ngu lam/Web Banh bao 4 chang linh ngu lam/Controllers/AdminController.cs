using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using Web_Banh_bao_4_chang_linh_ngu_lam.Models;

namespace Web_Banh_bao_4_chang_linh_ngu_lam.Controllers
{
    public class AdminController : Controller
    {
        DBBanhBaoTuanDatEntities db = new DBBanhBaoTuanDatEntities();

        // ===== LOGIN =====
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["AdminUser"] != null)
                return RedirectToAction("Dashboard");
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var admin = db.AdminUsers.FirstOrDefault(a => a.UserName == username && a.PasswordHash == password);
            if (admin != null)
            {
                Session["AdminUser"] = admin;
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }

        public ActionResult Logout()
        {
            Session["AdminUser"] = null;
            return RedirectToAction("Login");
        }

        // ===== DASHBOARD =====
        // ===== DASHBOARD =====
        public ActionResult Dashboard()
        {
            if (Session["AdminUser"] == null)
                return RedirectToAction("Login");

            var model = new AdminDashboardVM();

            // ===== TOTALS =====
            model.TotalProduct = db.Products.Count();
            model.TotalOrder = db.OrderProes.Count();
            model.TotalCustomer = db.Customers.Count();
            model.TotalRevenue = db.OrderProes.Sum(o => (decimal?)o.TotalAmount) ?? 0;

            // ===== TOP 5 PRODUCTS BY QUANTITY SOLD =====
            model.TopProducts = db.OrderDetails
                .GroupBy(d => d.Product.NamePro)
                .Select(g => new TopProductVM
                {
                    Product = g.Key,
                    TotalQty = g.Sum(x => x.Quantity ?? 0)
                })
                .OrderByDescending(x => x.TotalQty)
                .Take(5)
                .ToList();

            // ===== PROFIT TODAY =====
            var today = DateTime.Today;
            model.ProfitToday = db.OrderProes
                .Where(o => DbFunctions.TruncateTime(o.DateOrder) == today)
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;

            // ===== PROFIT THIS MONTH =====
            var monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            model.ProfitThisMonth = db.OrderProes
                .Where(o => DbFunctions.TruncateTime(o.DateOrder) >= monthStart)
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;

            // ===== BEST SELLING DAY =====
            var bestDay = db.OrderDetails
                .GroupBy(d => DbFunctions.TruncateTime(d.OrderPro.DateOrder))
                .Select(g => new
                {
                    Day = g.Key,
                    TotalQty = g.Sum(x => x.Quantity ?? 0)
                })
                .OrderByDescending(g => g.TotalQty)
                .FirstOrDefault();

            if (bestDay != null)
            {
                model.BestSellingDay = bestDay.Day?.ToString("dd/MM/yyyy");
                model.BestSellingDayQuantity = bestDay.TotalQty;
            }

            // ===== BEST SELLING MONTH =====
            var bestMonth = db.OrderDetails
                .GroupBy(d => new { d.OrderPro.DateOrder.Value.Year, d.OrderPro.DateOrder.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalQty = g.Sum(x => x.Quantity ?? 0)
                })
                .OrderByDescending(g => g.TotalQty)
                .FirstOrDefault();

            if (bestMonth != null)
            {
                model.BestSellingMonth = new DateTime(bestMonth.Year, bestMonth.Month, 1).ToString("MM/yyyy");
                model.BestSellingMonthQuantity = bestMonth.TotalQty;
            }

            return View(model);
        }

        // ===== PRODUCTS =====
        public ActionResult Products()
        {
            if (Session["AdminUser"] == null)
                return RedirectToAction("Login");

            var products = db.Products
                .Include(p => p.Category1)
                .OrderByDescending(p => p.ProductID)
                .ToList();

            return View(products);
        }

        public ActionResult CreateProduct()
        {
            if (Session["AdminUser"] == null)
                return RedirectToAction("Login");

            ViewBag.CateList = new SelectList(db.Categories.ToList(), "IDCate", "NameCate");
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(Product model, HttpPostedFileBase UploadImage)
        {
            if (model.IsFeatured == null)
                model.IsFeatured = false;

            if (ModelState.IsValid)
            {
                if (UploadImage != null)
                {
                    string fileName = Path.GetFileName(UploadImage.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                    UploadImage.SaveAs(path);
                    model.ImagePro = fileName;
                }

                db.Products.Add(model);
                db.SaveChanges();
                return RedirectToAction("Products");
            }

            ViewBag.CateList = new SelectList(db.Categories.ToList(), "IDCate", "NameCate", model.Category ?? 0);
            return View(model);
        }

        public ActionResult EditProduct(int id)
        {
            if (Session["AdminUser"] == null)
                return RedirectToAction("Login");

            Product product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            ViewBag.CateList = new SelectList(db.Categories.ToList(), "IDCate", "NameCate", product.Category ?? 0);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product model, HttpPostedFileBase UploadImage)
        {
            if (model.IsFeatured == null)
                model.IsFeatured = false;

            if (ModelState.IsValid)
            {
                Product product = db.Products.Find(model.ProductID);
                if (product == null)
                    return HttpNotFound();

                product.NamePro = model.NamePro;
                product.DescriptionPro = model.DescriptionPro;
                product.Category = model.Category;
                product.Price = model.Price;
                product.IsFeatured = model.IsFeatured;

                if (UploadImage != null)
                {
                    string fileName = Path.GetFileName(UploadImage.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                    UploadImage.SaveAs(path);
                    product.ImagePro = fileName;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Products");
            }

            ViewBag.CateList = new SelectList(db.Categories.ToList(), "IDCate", "NameCate", model.Category ?? 0);
            return View(model);
        }

        public ActionResult DeleteProduct(int id)
        {
            if (Session["AdminUser"] == null)
                return RedirectToAction("Login");

            var product = db.Products.Find(id);
            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }

            return RedirectToAction("Products");
        }

        // ===== ORDERS =====
        public ActionResult Orders()
        {
            if (Session["AdminUser"] == null)
                return RedirectToAction("Login");

            var orders = db.OrderProes
                            .OrderByDescending(o => o.DateOrder)
                            .ToList();

            return View(orders);
        }

        public ActionResult OrderDetail(int id)
        {
            if (Session["AdminUser"] == null)
                return RedirectToAction("Login");

            var order = db.OrderProes.Find(id);
            if (order == null) return HttpNotFound();

            return View(order);
        }

        [HttpPost]
        public ActionResult UpdateStatus(int id, string status)
        {
            if (Session["AdminUser"] == null)
                return RedirectToAction("Login");

            var order = db.OrderProes.Find(id);
            if (order != null)
            {
                order.Status = status;
                db.SaveChanges();
            }

            return RedirectToAction("Orders");
        }

        // ===== DISCOUNT =====
        public ActionResult Discount()
        {
            if (Session["AdminUser"] == null)
                return RedirectToAction("Login");

            var discount = db.MaGiamGias.OrderByDescending(d => d.CreatedAt).ToList();
            return View(discount);
        }

        public ActionResult CreateDiscount()
        {
            if (Session["AdminUser"] == null)
                return RedirectToAction("Login");

            return View(new MaGiamGia { IsActive = true, CreatedAt = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDiscount(MaGiamGia model)
        {
            // Check duplicate code
            if (db.MaGiamGias.Any(x => x.Code == model.Code))
                ModelState.AddModelError("", "Mã giảm giá đã tồn tại.");

            // Type 1 = Buy X Get Y
            if (model.Type == 1)
            {
                if (model.BuyQuantity.GetValueOrDefault() <= 0)
                    ModelState.AddModelError("", "Số lượng mua phải > 0.");

                if (model.FreeQuantity.GetValueOrDefault() <= 0)
                    ModelState.AddModelError("", "Số lượng tặng phải > 0.");
            }

            // Type 2 = Percent sale
            if (model.Type == 2)
            {
                if (model.PercentOff.GetValueOrDefault() <= 0 || model.PercentOff.GetValueOrDefault() > 90)
                    ModelState.AddModelError("", "Giảm giá phải từ 1% đến 90%.");
            }

            if (!ModelState.IsValid)
                return View(model);

            model.CreatedAt = DateTime.Now;
            model.IsActive = true;

            db.MaGiamGias.Add(model);
            db.SaveChanges();

            return RedirectToAction("Discount");
        }

        public ActionResult ToggleDiscount(int id)
        {
            if (Session["AdminUser"] == null)
                return RedirectToAction("Login");

            var discount = db.MaGiamGias.Find(id);
            if (discount != null)
            {
                discount.IsActive = !discount.IsActive;
                db.Entry(discount).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Discount");
        }
    }
}