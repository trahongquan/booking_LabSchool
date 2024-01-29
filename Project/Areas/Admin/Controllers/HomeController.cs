using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            var user = Session["user"] as account;
            if (user == null)
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }
            var dashboard = new DashboardViewModel(user);
            return View(dashboard);
        }
    }
}