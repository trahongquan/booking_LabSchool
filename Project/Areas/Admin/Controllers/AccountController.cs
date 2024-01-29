using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private readonly int ROLE_LEADER = 2;
        // GET: Admin/Account
        public ActionResult Index()
        {
            AccountViewModel accountViewModel = new AccountViewModel();
            return View(accountViewModel);
        }


        [HttpGet]
        public ActionResult addAccount()
        {
            AccountViewModel accountViewModel = new AccountViewModel();
            return View(accountViewModel);
        }


        [HttpPost]
        public ActionResult Add(AccountViewModel model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                using (DBContext dbContext = new DBContext())
                {
                    var role_id = form["role_id"];
                    if (role_id != null)
                    {
                        var user = dbContext.accounts.FirstOrDefault(m => m.username == model.account.username);
                        if (user != null)
                        {
                            if (user.is_deleted == true)
                            {
                                user.fullname = model.account.fullname;
                                user.password = model.account.password;
                                user.role_id = Convert.ToInt32(role_id);
                                user.is_deleted = false;
                                dbContext.SaveChanges();
                                return RedirectToAction("Index", "Account");
                            }
                            ViewBag.Message = "Tên đăng nhập đã tồn tại.";
                            return View("addAccount", model);
                        }
                        model.account.role_id = Convert.ToInt32(role_id);
                        dbContext.accounts.Add(model.account);
                        dbContext.SaveChanges();
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        ViewBag.Message = "Vui lòng chọn quyền tài khoản cho người dùng.";
                        return View("addAccount", model);
                    }
                }
            }
            return View();
        }


        [HttpGet]
        public ActionResult editAccount(int id)
        {
            using (DBContext dbContext = new DBContext())
            {
                AccountViewModel accountViewModel = new AccountViewModel();
                var account = dbContext.accounts.FirstOrDefault(x => x.id == id);
                if (account != null)
                {
                    accountViewModel.account = account;
                    return View(accountViewModel);
                }
                else
                {
                    return View("Index");
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(AccountViewModel model, FormCollection form)
        {
            using (DBContext dbContext = new DBContext())
            {
                var account = dbContext.accounts.FirstOrDefault(m => model.account.id == m.id);
                if (account != null)
                {
                    account.password = model.account.password;
                    account.fullname = model.account.fullname;
                    account.role_id = Convert.ToInt32(form["role_id"].ToString());
                    dbContext.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (DBContext dbContext = new DBContext())
            {
                try
                {
                    var account = dbContext.accounts.FirstOrDefault(y => y.id == id);
                    if (account != null)
                    {
                        if (account.role_id == ROLE_LEADER)
                        {
                            var room = dbContext.rooms.
                                FirstOrDefault(r => r.is_deleted == false && r.leader_id == account.id);
                            if (room != null)
                            {
                                room.leader_id = null;
                            }
                        }
                        account.is_deleted = true;

                        var bookingToRemove = dbContext.bookings.Where(b => b.user_id == account.id);
                        dbContext.bookings.RemoveRange(bookingToRemove);
                        dbContext.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ViewBag.Message = "Không thể xóa tài khoản này.";
                    return View("Index", new AccountViewModel());
                }
            }
        }
    }
}