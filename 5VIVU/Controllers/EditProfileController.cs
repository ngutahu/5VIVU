using _5VIVU.Data;
using _5VIVU.Models;
using _5VIVU.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.RegularExpressions;

namespace _5VIVU.Controllers
{
    public class EditProfileController : Controller
    {
        private readonly _5vivuBanveContext _context;

        public EditProfileController(_5vivuBanveContext context)
        {
            _context = context;
        }

        [Authentication]
        public IActionResult Index(int? id)
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            var user = _context.AppUsers.FirstOrDefault(t => t.Id == id);
            if (user != null)
            {
                var result = new UserVM()


                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Address = user.Address,
                    Birthday = user.Birthday,
                    Phone = user.Phone,
                    Email = user.Email,
                    Wallet = user.Wallet,
                    Cmnd = user.Cmnd
                };


                return View(result);
            }

            // Trả về một danh sách trống nếu không tìm thấy user
            return View();
        }
        [Authentication]
        [HttpPost]
        public IActionResult SaveChanges(UserVM model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.Phone) && !Regex.IsMatch(model.Phone, @"^0\d{9}$"))
                {
                    ModelState.AddModelError("Phone", "Số điện thoại phải chứa đúng 10 số và bắt đầu bằng số 0.");
                    return View("Index", model); // Trả về view với model hiện tại nếu có lỗi
                }

                if (!string.IsNullOrEmpty(model.FullName) && !System.Text.RegularExpressions.Regex.IsMatch(model.FullName, @"^[a-zA-ZÀ-ỹ\s]*$"))
                {
                    ModelState.AddModelError("FullName", "Họ và tên chỉ được nhập chữ và khoảng trắng.");
                    return View("Index", model); // Trả về view với model hiện tại nếu có lỗi
                }
                var currentDateTime = DateTime.Now;
                var today = new DateOnly(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day);
                if (model.Birthday > DateTime.Today)
                {
                    ModelState.AddModelError("Birthday", "Ngày sinh không được lớn hơn ngày hiện tại.");
                    return View("Index", model); // Trả về view với model hiện tại nếu có lỗi
                }
                if (!string.IsNullOrEmpty(model.Cmnd))
                {
                    if (!Regex.IsMatch(model.Cmnd, @"^(?:\d{9,12}|[a-zA-Z]\d{7})$"))
                    {
                        ModelState.AddModelError("Cmnd", "Số căn cước, chứng minh nhân dân hoặc passport không hợp lệ.");
                        return View("Index", model); // Trả về view với model hiện tại nếu có lỗi
                    }
                }

                var user = _context.AppUsers.FirstOrDefault(u => u.Id == model.Id);
                if (user != null)
                {
                    user.FullName = model.FullName;
                    user.Address = model.Address;
                    user.Birthday = model.Birthday;
                    user.Phone = model.Phone;
                    user.Email = model.Email;
                    user.Wallet = model.Wallet;
                    user.Cmnd = model.Cmnd;
                    user.Id = model.Id;

                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Thông tin của bạn đã được cập nhật thành công!";
                    return RedirectToAction("Index", new { id = model.Id });
                }


            }
            // Trả về view với model hiện tại nếu có lỗi xảy ra
            return View("Index", model);
        }

    }
}