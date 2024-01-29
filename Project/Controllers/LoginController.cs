using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public ActionResult login(account account)
        {
            using (DBContext db = new DBContext())
            {
                var user = db.accounts.FirstOrDefault(u => u.username.Equals(account.username) && u.password.Equals(account.password));
                if (user == null)
                {
                    ViewBag.Message = "Tên đăng nhập hoặc mật khẩu không đúng.";
                    return View("Index");
                }
                Session["user"] = user;
                if (user.role.role_name.Equals("ADMIN") || user.role.role_name.Equals("LEADER_CLASS"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

    }
}