using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Models
{
    public class BookingViewModel
    {
        private readonly int ROLE_ADMIN = 1;
        private readonly int ROLE_LEADER = 1;
        private readonly int ROLE_TEACHER = 3;
        public List<booking> bookingListClient { get; set; }

        public List<booking> bookingListAdmin { get; set; }
        public booking booking { get; set; }
        public bool isAdmin { get; set; }
        public string shift { get; set; }

        public BookingViewModel()
        {
            booking = new booking();
        }

        public List<SelectListItem> getListTeacher()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (var ctx = new DBContext())
            {
                ctx.accounts.ForEach(item =>
                {
                    if (item.is_deleted == false && item.role_id == ROLE_TEACHER)
                    {
                        list.Add(new SelectListItem { Value = item.id.ToString(), Text = item.fullname });
                    }
                });
            }
            return list;
        }

        public List<SelectListItem> getListRoom(account user)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (var ctx = new DBContext())
            {
                if (user.role_id == ROLE_ADMIN)
                {
                    ctx.rooms.ForEach(item =>
                    {
                        if (item.is_deleted == false)
                        {
                            list.Add(new SelectListItem { Value = item.id.ToString(), Text = item.room_name });
                        }
                    });
                }
                else if (user.role_id == ROLE_LEADER)
                {
                    var room = ctx.rooms.FirstOrDefault(item => item.is_deleted == false && item.leader_id == user.id);
                    if (room != null)
                        list.Add(new SelectListItem { Value = room.id.ToString(), Text = room.room_name });
                }

            }
            return list;
        }

        public BookingViewModel(account currentUser)
        {
            using (var ctx = new DBContext())
            {
                if (currentUser.role_id == ROLE_ADMIN)
                {
                    bookingListAdmin = ctx.bookings.ToList();
                    isAdmin = true;
                }
                else
                {
                    bookingListAdmin = ctx.rooms.Where(r => r.is_deleted == false && r.leader_id == currentUser.id)
                        .SelectMany(r => r.bookings).ToList();
                    isAdmin = false;
                }
            }
        }


        public string getTeacherName(int? teachId)
        {
            string teacherName = string.Empty;
            using (var ctx = new DBContext())
            {
                var teacher = ctx.accounts.FirstOrDefault(acc => acc.is_deleted == false && acc.id == teachId);
                if (teacher != null) return teacher.fullname;
            }
            return teacherName;
        }

        public string getRoomName(int? roomId)
        {
            string roomName = string.Empty;
            using (var ctx = new DBContext())
            {
                var room = ctx.rooms.FirstOrDefault(r => r.id == roomId);
                if (room != null) roomName = room.room_name;
            }
            return roomName;
        }

        public string dateFormat(DateTime? date)
        {
            DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
            string str = date.HasValue ? date.Value.ToString(myDTFI.LongDatePattern.Replace("dddd, ", "")) : string.Empty;
            return str;
        }
    }
}