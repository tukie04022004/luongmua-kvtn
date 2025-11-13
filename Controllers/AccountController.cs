using Microsoft.AspNetCore.Mvc;
using KttvKvtnWeb.Data;
using KttvKvtnWeb.Models;

namespace KttvKvtnWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context) => _context = context;

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string userid, string matkhau)
        {
            var user = _context.Users.FirstOrDefault(u => u.Userid == userid && u.Matkhau == matkhau);
            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.Userid);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}