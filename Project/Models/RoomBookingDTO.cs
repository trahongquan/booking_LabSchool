using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class RoomBookingDTO
    {
        public int RoomId { get; set; }
        public account userBookMorning { get; set; }
        public account userBookEvening { get; set; }
        public string RoomName { get; set; }
        public string RoomLeader { get; set; }
        public DateTime? BookingDate { get; set; }
        public bool IsMorningAvailable { get; set; }
        public bool IsEveningAvailable { get; set; }
        public bool ConfirmStatus { get; set; }

    }
}