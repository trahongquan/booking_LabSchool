using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Admin.Controllers
{
    public class BookingController : Controller
    {
        private static readonly int ROLE_TEACH = 3;
        private static readonly int ROLE_ADMIN = 1;
        private static readonly int ROLE_LEADER = 2;

        private readonly byte WAIT_CONFIRM = 0;
        private readonly byte ACCEPTED = 1;
        private readonly byte CANCEL = 2;
        // GET: Admin/Booking
        public ActionResult Index()
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                using (DBContext ctx = new DBContext())
                {
                    var bookingModel = new BookingViewModel(user);
                    return View(bookingModel);
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        public ActionResult BookingRoomForTeacher()
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                var bookingModel = new BookingViewModel(user);
                return View(bookingModel);
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        // BookingForTeach


        [HttpGet]
        public JsonResult StatisticsBooking(string from, string to)
        {
            var user = Session["user"] as account;
            if (user != null)
            {
                if (DateTime.TryParse(from, out DateTime _from) && DateTime.TryParse(to, out DateTime _to))
                {
                    var ctx = new DBContext();
                    ctx.Configuration.ProxyCreationEnabled = false;  // tranh bi loi
                    var availableRooms = ctx.rooms
                        .Where(r => !r.is_deleted)
                        .Select(room => new StatisticsDTO
                        {
                            RoomId = room.id,
                            RoomName = room.room_name,
                            RoomLeader = ctx.accounts.FirstOrDefault(acc => acc.id == room.leader_id).fullname,
                             
                            countMorningBook = ctx.bookings.Count(b => b.room_id == room.id && b.booking_status == false && (b.booking_date >= _from && b.booking_date <= _to)),

                            countEveningBook = ctx.bookings.Count(b => b.room_id == room.id && b.booking_status == true && (b.booking_date >= _from && b.booking_date <= _to))
                        })
                        //.Where(room => !(room.IsEveningAvailable && room.IsMorningAvailable))
                        .ToList();

                    return Json(new { availableRooms }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Error = "Lỗi định dạng ngày." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Error = "Vui lòng đăng nhập" }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult BookingForTeach(booking booking, string shift)
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                bool _shift = (shift == "t");
                using (var ctx = new DBContext())
                {
                    var isBooking = ctx.bookings.FirstOrDefault(item => item.booking_date == booking.booking_date && item.booking_status == _shift
                    && item.room_id == booking.room_id);
                    bool check = false;
                    if (isBooking != null)
                    {
                        if (isBooking.confirmation_status == 1)
                        {
                            ViewBag.Message = "Lịch này đã được giáo viên khác đặt rồi!";
                            check = true;
                        }
                    }
                    var duplicateBook = ctx.bookings.FirstOrDefault(item => item.booking_date == booking.booking_date && item.booking_status == _shift && item.user_id == booking.user_id);
                    if(duplicateBook != null)
                    {
                        check = true;
                        ViewBag.Message = "Giáo viên này đã đặt lịch vào buổi này rồi!";
                    }

                    if (check)
                    {
                        BookingViewModel bookingModel = new BookingViewModel(user);
                        return View("BookingRoomForTeacher", bookingModel);
                    }
                    booking.confirmation_status = 1;
                    booking.booking_status = _shift;
                    ctx.bookings.Add(booking);
                    ctx.SaveChanges();
                    return RedirectToAction("List");
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        public ActionResult List()
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                return View();
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        public ActionResult Statistics()
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                return View();
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        public ActionResult Accept(int bid)
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                using (DBContext ctx = new DBContext())
                {
                    var booking = ctx.bookings.FirstOrDefault(b => b.id == bid);
                    if (booking != null)
                    {
                        booking.confirmation_status = ACCEPTED;
                        ctx.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        public ActionResult Reject(int bid)
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                using (DBContext ctx = new DBContext())
                {
                    var booking = ctx.bookings.FirstOrDefault(b => b.id == bid);
                    if (booking != null)
                    {
                        booking.confirmation_status = CANCEL;
                        ctx.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

    }
}