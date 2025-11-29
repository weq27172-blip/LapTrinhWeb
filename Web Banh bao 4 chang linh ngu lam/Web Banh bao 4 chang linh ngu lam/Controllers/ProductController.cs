using System;
using System.IO;
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
        public ActionResult Index(int? categoryId)
        {
            var categories = db.Categories.ToList();

            var productsQuery = db.Products.AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                productsQuery = productsQuery.Where(p => p.Category == categoryId.Value);
            }

            var viewModel = new ProductFilterVM
            {
                Categories = categories,
                Products = productsQuery.ToList(),
                SelectedCategoryId = categoryId
            };

            return View(viewModel);
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

        // ===== CREATE PRODUCT (FRONTEND) =====
        public ActionResult Create()
        {
            ViewBag.CateList = new SelectList(db.Categories, "IDCate", "NameCate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product model, HttpPostedFileBase UploadImage)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CateList = new SelectList(db.Categories, "IDCate", "NameCate");
                return View(model);
            }

            // Handle image upload
            if (UploadImage != null && UploadImage.ContentLength > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(UploadImage.FileName);
                string path = Server.MapPath("~/Content/Images/" + fileName);
                UploadImage.SaveAs(path);
                model.ImagePro = fileName;
            }
            else
            {
                model.ImagePro = "noimage.jpg"; // default image
            }

            db.Products.Add(model);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // ===== EDIT PRODUCT (FRONTEND) =====
        public ActionResult Edit(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            ViewBag.CateList = new SelectList(db.Categories, "IDCate", "NameCate", product.Category);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model, HttpPostedFileBase UploadImage)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CateList = new SelectList(db.Categories, "IDCate", "NameCate", model.Category);
                return View(model);
            }

            var product = db.Products.Find(model.ProductID);
            if (product == null)
                return HttpNotFound();

            product.NamePro = model.NamePro;
            product.DescriptionPro = model.DescriptionPro;
            product.Category = model.Category;
            product.Price = model.Price;
            product.IsFeatured = model.IsFeatured;

            if (UploadImage != null && UploadImage.ContentLength > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(UploadImage.FileName);
                string path = Server.MapPath("~/Content/Images/" + fileName);
                UploadImage.SaveAs(path);
                product.ImagePro = fileName;
            }

            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // ===== DELETE PRODUCT =====
        public ActionResult Delete(int id)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}