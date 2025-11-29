using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_Banh_bao_4_chang_linh_ngu_lam.Models;

namespace Web_Banh_bao_4_chang_linh_ngu_lam.Controllers
{
    public class ProductController : Controller
    {
        DBBanhBaoTuanDatEntities db = new DBBanhBaoTuanDatEntities();

        // ===== LIST PRODUCTS =====
        public ActionResult Index(int? categoryId, int page = 1, string sort = "default")
        {
            int pageSize = 12; // 12 products per page

            // Load categories for sidebar
            var categories = db.Categories.ToList();

            // Base query: only active products
            var productsQuery = db.Products.Where(p => !p.IsDeleted);

            // Filter by category if selected
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                productsQuery = productsQuery.Where(p => p.Category == categoryId.Value);
            }

            // Apply sorting
            switch (sort)
            {
                case "az":
                    productsQuery = productsQuery.OrderBy(p => p.NamePro);
                    break;
                case "za":
                    productsQuery = productsQuery.OrderByDescending(p => p.NamePro);
                    break;
                case "price_asc":
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                case "bestseller":
                    productsQuery = productsQuery.OrderByDescending(p =>
                        db.OrderDetails
                          .Where(od => od.IDProduct == p.ProductID)
                          .Sum(od => (int?)od.Quantity) ?? 0
                    );
                    break;
                default:
                    // Default order to prevent Skip exception
                    productsQuery = productsQuery.OrderBy(p => p.ProductID);
                    break;
            }

            // Total products for pagination
            int totalProducts = productsQuery.Count();

            // Paging
            var products = productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Prepare ViewModel
            var vm = new ProductFilterVM
            {
                Categories = categories,
                Products = products,
                SelectedCategoryId = categoryId,
                CurrentPage = page,
                PageSize = pageSize,
                TotalProducts = totalProducts
            };

            ViewBag.Sort = sort; // for UI dropdown

            return View(vm);
        }

        // ===== PRODUCT DETAILS =====
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products.Find(id.Value);
            if (product == null)
                return HttpNotFound();

            // Load reviews
            var reviews = db.Reviews
                .Where(r => r.ProductID == id.Value)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
            ViewBag.Reviews = reviews;

            return View(product);
        }

        // ===== ADD REVIEW =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(int productId, int rating, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                TempData["Message"] = "Bạn phải nhập nội dung đánh giá!";
                return RedirectToAction("Details", new { id = productId });
            }

            Review rv = new Review
            {
                ProductID = productId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            db.Reviews.Add(rv);
            db.SaveChanges();

            TempData["Message"] = "Đã gửi đánh giá thành công!";
            return RedirectToAction("Details", new { id = productId });
        }
    }
}