using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class StatisticsDTO
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomLeader { get; set; }
        public DateTime? BookingDate { get; set; }
        public int countMorningBook { get; set; }
        public int countEveningBook { get; set; }
    }
}