using Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Admin.Controllers
{

    public class RoomController : Controller
    {
        private readonly int ROLE_TEACH = 3;
        private readonly int ROLE_ADMIN = 1;
        private readonly int ROLE_LEADER_CLASS = 2;
        // GET: Admin/Room
        public ActionResult Index()
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                using (DBContext db = new DBContext())
                {
                    RoomViewModel model = new RoomViewModel(user);
                    return View(model);
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        public ActionResult add()
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                using (DBContext db = new DBContext())
                {
                    RoomViewModel model = new RoomViewModel(user);
                    return View(model);
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        [HttpPost]
        public ActionResult Add(room room, FormCollection form)
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                using (DBContext ctx = new DBContext())
                {
                    var leader_id = form["leader"];
                    var lid = Convert.ToInt32(leader_id.ToString());
                    var u = ctx.accounts.FirstOrDefault(userLeader => userLeader.id == lid);
                    var r = ctx.rooms.FirstOrDefault(m => m.room_name.Equals(room.room_name));
                    if (r != null)
                    {
                        if (r.is_deleted == true)
                        {
                            r.capacity = room.capacity;
                            if (lid > 0)
                            {
                                r.leader_id = lid;
                            }
                            else
                            {
                                r.leader_id = null;
                            }
                            r.is_deleted = false;
                            ctx.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        ViewBag.Message = "Tên phòng thí nghiệm đã tồn tại.";
                        RoomViewModel model = new RoomViewModel(user);
                        return View("add", model);
                    }
                    else
                    {

                        if (lid > 0)
                        {
                            room.leader_id = lid;
                        }
                        else
                        {
                            room.leader_id = null;
                        }
                        ctx.rooms.Add(room);
                        ctx.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        [HttpGet]
        public ActionResult edit(int id)
        {

            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                using (DBContext db = new DBContext())
                {
                    RoomViewModel model = new RoomViewModel(user);
                    var room = db.rooms.FirstOrDefault(r => r.id == id);
                    model.room = room;
                    return View(model);
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        [HttpPost]
        public ActionResult edit(room room, FormCollection form)
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                var model = new RoomViewModel(user);
                using (DBContext ctx = new DBContext())
                {
                    var r = ctx.rooms.FirstOrDefault(m => m.id == room.id);
                    if (r != null)
                    {
                        var roomExist = ctx.rooms.FirstOrDefault(m => m.id != room.id && m.room_name.Equals(room.room_name));
                        if (roomExist != null)
                        {
                            ViewBag.Message = "Vui lòng chọn tên phòng thí nghiệm khác.";
                            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
                            {
                                model.room = room;
                                return View("edit", model);
                            }
                        }
                        else
                        {
                            var leader_id = form["leader"];
                            var lid = Convert.ToInt32(leader_id.ToString());
                            if (lid > 0)
                            {
                                r.leader_id = lid;
                            }
                            else
                            {
                                r.leader_id = null;
                            }
                            r.room_name = room.room_name;
                            r.capacity = room.capacity;
                            r.location = room.location;
                            ctx.SaveChanges();
                        }
                    }
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                using (DBContext ctx = new DBContext())
                {
                    var room = ctx.rooms.FirstOrDefault(o => o.id == id);
                    if (room != null)
                    {
                        room.is_deleted = true;
                        room.leader_id = null;

                        var bookingToRemove = ctx.bookings.Where(b => b.room_id == id);
                        ctx.bookings.RemoveRange(bookingToRemove);
                        ctx.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }
    }
}