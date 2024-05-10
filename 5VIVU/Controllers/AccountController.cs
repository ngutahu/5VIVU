using Microsoft.AspNetCore.Mvc;
using _5VIVU.Data;
using _5VIVU.Models;

namespace _5VIVU.Controllers
{
    public class AccountController : Controller
    {
        private readonly _5vivuBanveContext _context;

        public AccountController(_5vivuBanveContext context)
        {
            _context = context;
        }

        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin userlogin)
        {
            if (ModelState.IsValid)
            {
                var user = _context.AppUsers.FirstOrDefault(u => u.Email == userlogin.Email && u.Password == userlogin.Password);

                if (user != null)
                {
                    // Kiểm tra vai trò của người dùng
                    if (user.Role == 1) // Admin
                    {
                        // Đăng nhập thành công cho Admin, thiết lập session và chuyển hướng tới trang Admin Dashboard
                        HttpContext.Session.SetString("UserName", user.Email);
                        HttpContext.Session.SetInt32("Role", user.Role);
                        return RedirectToAction("ListBookings", "Admin");
                    }
                    else if (user.Role == 0) // User
                    {
                        // Đăng nhập thành công cho User, thiết lập session và chuyển hướng tới trang User Dashboard
                        HttpContext.Session.SetString("UserName", user.Email);
                        HttpContext.Session.SetInt32("Role", user.Role);
                        HttpContext.Session.SetInt32("Id", user.Id);

                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Invalid email or password");
                return View();
            }
            // Nếu xác thực thất bại hoặc dữ liệu không hợp lệ, thêm thông báo lỗi và hiển thị lại trang đăng nhập
            ModelState.AddModelError("", "Invalid email or password");
            return View();
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("Role");
            return RedirectToAction("Index", "Home");
        }
    }
}