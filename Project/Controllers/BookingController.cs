using Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class BookingController : Controller
    {
        private readonly byte WAIT_CONFIRM = 0;
        private readonly byte ACCEPTED = 1;
        private readonly byte CANCEL = 2;
        // GET: Booking
        public ActionResult Index()
        {
            var user = Session["user"] as account;
            if (user != null)
            {
                using (var ctx = new DBContext())
                {
                    var list = ctx.bookings.Where(m => m.user_id == user.id).ToList();
                    var bookingModel = new BookingViewModel();
                    bookingModel.bookingListClient = list;
                    return View(bookingModel);
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }


        [HttpGet]
        public JsonResult List(string date)
        {
            var user = Session["user"] as account;
            if (user != null)
            {
                if (DateTime.TryParse(date, out DateTime result))
                {
                    var ctx = new DBContext();
                    ctx.Configuration.ProxyCreationEnabled = false;  // tranh bi loi
                    var availableRooms = ctx.rooms
                        .Where(r => !r.is_deleted)
                        .Select(room => new RoomBookingDTO
                        {
                            RoomId = room.id,
                            RoomName = room.room_name,
                            BookingDate = result,
                            RoomLeader = ctx.accounts.FirstOrDefault(acc => acc.id == room.leader_id).fullname,
                            userBookMorning = ctx.bookings.Where(b => b.room_id == room.id && b.booking_status == false && b.confirmation_status == ACCEPTED && b.booking_date == result).Select(acc => acc.account).FirstOrDefault(),

                            userBookEvening = ctx.bookings.Where(b => b.room_id == room.id && b.booking_status == true && b.confirmation_status == ACCEPTED && b.booking_date == result).Select(acc => acc.account).FirstOrDefault(),

                            IsMorningAvailable = ctx.bookings.Any(b => b.room_id == room.id && b.booking_status == false && b.confirmation_status == ACCEPTED && b.booking_date == result),

                            IsEveningAvailable = ctx.bookings.Any(b => b.room_id == room.id && b.booking_status == true && b.confirmation_status == ACCEPTED && b.booking_date == result)
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
        public JsonResult addBooking(int room_id, int user_id, int shift, string date)
        {

            string s = "";
            using (var ctx = new DBContext())
            {

                DateTime bookingDate = DateTime.Parse(date); // Convert the date string to DateTime
                var _shift = (shift == 1) ? true : false;
                var isvalid = checkDuplicate(user_id, bookingDate, _shift);
                if (isvalid)
                {
                    s = "Bạn không thể đặt trùng 1 ca trong 1 ngày được!";
                }
                else
                {
                    if (checkRoomIsValid(room_id, _shift, bookingDate))
                    {
                        s = $"Ca này ở phòng này đã được người phụ trách xác nhận cho người khác";
                    }
                    else
                    {
                        var b = new booking
                        {
                            user_id = user_id,
                            room_id = room_id,
                            booking_status = (shift == 0) ? false : true,
                            booking_date = Convert.ToDateTime(date),
                            confirmation_status = WAIT_CONFIRM
                        };
                        ctx.bookings.Add(b);
                        ctx.SaveChanges();
                        s = "Thành công!";
                    }
                }
            }
            return Json(new { Mess = s });
        }
        private bool checkRoomIsValid(int room_id, bool shift, DateTime date_booking)
        {
            using (var ctx = new DBContext())
            {
                // Check for duplicate booking on the same date and shift
                bool isValid = ctx.bookings.Any(b =>
                    b.room_id == room_id &&
                    b.booking_date == date_booking &&
                    b.booking_status == shift &&
                    b.confirmation_status == ACCEPTED);
                return isValid;
            }
        }

        private bool checkDuplicate(int user_id, DateTime date_booking, bool shift)
        {
            using (var ctx = new DBContext())
            {
                // Check for duplicate booking on the same date and shift
                bool hasDuplicate = ctx.bookings.Any(b =>
                    b.user_id == user_id &&
                    b.booking_date == date_booking &&
                    b.booking_status == shift);
                return hasDuplicate;
            }
        }
    }
}