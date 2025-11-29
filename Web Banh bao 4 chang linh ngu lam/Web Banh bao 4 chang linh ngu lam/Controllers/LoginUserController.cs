using System.Linq;
using System.Web.Mvc;
using Web_Banh_bao_4_chang_linh_ngu_lam.Models;

namespace Web_Banh_bao_4_chang_linh_ngu_lam.Controllers
{
    public class LoginUserController : Controller
    {
        DBBanhBaoTuanDatEntities db = new DBBanhBaoTuanDatEntities();

        // --- Đăng ký ---
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Customer cus)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(cus);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(cus);
        }

        // --- Đăng nhập ---
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string LoginIdentifier, string Password)
        {
            // --- Kiểm tra admin ---
            var admin = db.AdminUsers.FirstOrDefault(a =>
                a.UserName == LoginIdentifier && a.PasswordHash == Password);

            if (admin != null)
            {
                Session["AdminUser"] = admin.UserName;
                return RedirectToAction("Dashboard", "Admin");
            }

            // --- Kiểm tra khách hàng ---
            var user = db.Customers.FirstOrDefault(c =>
                (c.EmailCus == LoginIdentifier || c.NameCus == LoginIdentifier)
                && c.PassCus == Password);

            if (user != null)
            {
                Session["UserName"] = user.NameCus;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai tên đăng nhập/email hoặc mật khẩu!";
            return View();
        }

        // --- Logout ---
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // --- FORGOT PASSWORD VIEW ---
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string EmailCus)
        {
            if (string.IsNullOrEmpty(EmailCus))
            {
                ViewBag.Error = "Vui lòng nhập email của bạn!";
                return View();
            }

            var user = db.Customers.FirstOrDefault(x => x.EmailCus == EmailCus);
            if (user == null)
            {
                ViewBag.Error = "Email không tồn tại!";
                return View();
            }

            // --- In a real app, generate a reset token and send email ---
            // For demo, we just show a success message
            ViewBag.Message = "Một liên kết đặt lại mật khẩu đã được gửi đến email của bạn (demo).";

            return View();
        }
    }
}