using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{

    public class RoomEquipmentViewModel
    {
        public List<equipment> equipmentList { get; set; }
        public List<room> roomList { get; set; }
        public Dictionary<room, List<equipment>> roomEquipment { get; set; }

        public RoomEquipmentViewModel()
        {
            using (DBContext ctx = new DBContext())
            {
                equipmentList = ctx.equipments.Where(m => m.status == true && m.is_deleted == false).ToList();
                roomList = ctx.rooms.Where(r => r.is_deleted == false).ToList();
                roomEquipment = ctx.rooms.Where(room => room.is_deleted == false).ToList()
                    .ToDictionary(
                    room => room,
                    room => room.equipments.Where(eq => eq.is_deleted == false).ToList()
                    );
            }
        }

        public string getEquipmentName(int? id)
        {
            using (DBContext ctx = new DBContext())
            {
                var item = ctx.equipments.FirstOrDefault(x => x.id == id && x.status == true);
                if (item != null)
                {
                    return $"{item.equipment_type} - {item.origin}";
                }
            }
            return "";
        }
        public string getRoomName(int? id)
        {
            using (DBContext ctx = new DBContext())
            {
                var item = ctx.rooms.FirstOrDefault(m => m.id == id);
                if (item != null)
                {
                    return $"{item.room_name}";
                }
            }
            return "";
        }

    }
}