using System;
using System.Web.Mvc;
using Web_Banh_bao_4_chang_linh_ngu_lam.Models;

namespace Web_Banh_bao_4_chang_linh_ngu_lam.Controllers
{
    public class ReviewController : Controller
    {
        private DBBanhBaoTuanDatEntities db = new DBBanhBaoTuanDatEntities();

        [HttpPost]
        public ActionResult AddReview(int productId, int? customerId, int rating, string comment)
        {
            // Thêm review trực tiếp bằng SQL
            db.Database.ExecuteSqlCommand(
                "INSERT INTO Review (ProductID, CustomerID, Rating, Comment, CreatedAt) VALUES (@p0,@p1,@p2,@p3,GETDATE())",
                productId,
                (object)customerId ?? DBNull.Value, // Xử lý CustomerID có thể null
                rating,
                comment
            );
            // Chuyển hướng về trang chi tiết sản phẩm
            return RedirectToAction("Details", "Product", new { id = productId });
        }
    }
}