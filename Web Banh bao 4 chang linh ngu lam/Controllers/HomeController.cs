using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Web_Banh_bao_4_chang_linh_ngu_lam.Models;

namespace Web_Banh_bao_4_chang_linh_ngu_lam.Controllers
{
    public class HomeController : Controller
    {
        private DBBanhBaoTuanDatEntities db = new DBBanhBaoTuanDatEntities();

        // ============================
        // 🏠 TRANG CHỦ
        // ============================
        public ActionResult Index(int page = 1, int pageSize = 8)
        {
            // 🔹 Get only FEATURED products that are not deleted
            var featuredProducts = db.Products
                .Where(p => !p.IsDeleted && p.IsFeatured)
                .OrderByDescending(p => p.ProductID)
                .Take(8)
                .ToList();

            // 🔹 Featured / Best seller product for hero section
            var mostSold = db.OrderDetails
                .GroupBy(od => od.IDProduct)
                .Select(g => new { ProductID = g.Key, TotalSold = g.Sum(x => x.Quantity ?? 0) })
                .OrderByDescending(x => x.TotalSold)
                .FirstOrDefault();

            Product featuredProduct = null;
            if (mostSold != null)
            {
                featuredProduct = db.Products
                    .FirstOrDefault(p => p.ProductID == mostSold.ProductID && !p.IsDeleted);
            }
            ViewBag.FeaturedProduct = featuredProduct;

            // 🔹 Ratings for product cards
            ViewBag.AllReviews = db.Reviews
                .Select(r => new { r.ProductID, r.Rating })
                .ToList();

            // Return only featured products
            return View(featuredProducts);
        }
        // ============================
        // 📦 CHI TIẾT SẢN PHẨM
        // ============================
        public ActionResult Details(int id)
        {
            var product = db.Products
                .Include(p => p.Reviews)
                .Where(p => !p.IsDeleted && p.ProductID == id)
                .FirstOrDefault();

            if (product == null)
                return HttpNotFound();

            ViewBag.Reviews = product.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            // Example: guest user (ID = 1). Replace with login ID if needed.
            ViewBag.CustomerID = 1;

            return View(product);
        }

        // ============================
        // 💬 GỬI ĐÁNH GIÁ
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(int productId, int? customerId, int rating, string comment)
        {
            if (rating < 1 || rating > 5)
            {
                TempData["Error"] = "Điểm đánh giá không hợp lệ (1–5 sao).";
                return RedirectToAction("Details", new { id = productId });
            }

            var product = db.Products
                .Where(p => p.ProductID == productId && !p.IsDeleted)
                .FirstOrDefault();

            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("Index");
            }

            Review review = new Review
            {
                ProductID = productId,
                CustomerID = customerId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            db.Reviews.Add(review);
            db.SaveChanges();

            TempData["Success"] = "Cảm ơn bạn đã đánh giá sản phẩm!";
            return RedirectToAction("Details", new { id = productId });
        }

        // ============================
        // 🧹 GIẢI PHÓNG TÀI NGUYÊN
        // ============================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}