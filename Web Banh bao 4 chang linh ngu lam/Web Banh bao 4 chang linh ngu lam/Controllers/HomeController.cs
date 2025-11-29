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

        // 🏠 Trang chủ: hiển thị sản phẩm + review trung bình
        public ActionResult Index()
        {
            // Get all products
            var products = db.Products.ToList();

            // Get most sold product
            var mostSoldProduct = db.OrderDetails
                .GroupBy(od => od.ID)
                .Select(g => new
                {
                    ProductID = g.Key,
                    TotalSold = g.Sum(x => x.Quantity ?? 0)
                })
                .OrderByDescending(x => x.TotalSold)
                .FirstOrDefault();

            Product featuredProduct = null;
            if (mostSoldProduct != null)
            {
                featuredProduct = db.Products.Find(mostSoldProduct.ProductID);
            }

            // Pass both to view using ViewBag
            ViewBag.FeaturedProduct = featuredProduct;

            return View(products); // Model is IEnumerable<Product>
        }

        // 📦 Chi tiết sản phẩm
        public ActionResult Details(int id)
        {
            var product = db.Products
                .Include(p => p.Reviews)
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
                return HttpNotFound();

            // Truyền danh sách đánh giá vào ViewBag để View sử dụng
            // Đã có ở product.Reviews, nhưng nếu View dùng ViewBag.Reviews, cần thêm:
            ViewBag.Reviews = product.Reviews.OrderByDescending(r => r.CreatedAt).ToList();

            // Thiết lập CustomerID (Giả định: 1 là khách vãng lai, hoặc thay bằng ID thật nếu đăng nhập)
            // Cần thiết cho form đánh giá trong View
            ViewBag.CustomerID = 1;

            return View(product);
        }

        // 💬 Gửi đánh giá (POST)
        // **Đã sửa lỗi tên tham số để khớp với model (CustomerID thay UserName, CreatedAt thay Date)**
        // Tham số customerId phải là int? nếu trường CustomerID trong DB là NULLABLE.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(int productId, int? customerId, int rating, string comment)
        {
            if (rating < 1 || rating > 5)
            {
                TempData["Error"] = "Điểm đánh giá không hợp lệ (1-5 sao).";
                return RedirectToAction("Details", new { id = productId });
            }

            // Kiểm tra sản phẩm có tồn tại không
            var product = db.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("Index");
            }

            // Thêm review mới
            var review = new Review
            {
                ProductID = productId,
                // Sử dụng CustomerID từ form
                CustomerID = customerId,
                Rating = rating,
                Comment = comment,
                // Sử dụng CreatedAt để khớp với Entity Model
                CreatedAt = DateTime.Now
            };

            db.Reviews.Add(review);
            db.SaveChanges();

            TempData["Success"] = "Cảm ơn bạn đã đánh giá sản phẩm!";
            return RedirectToAction("Details", new { id = productId });
        }

        // 🧹 Giải phóng tài nguyên
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