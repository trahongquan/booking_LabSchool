using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class DashboardViewModel
    {
        public int numberAccount { get; set; }
        public int numberRoom { get; set; }
        public int numberEquipment { get; set; }
        public int numberBooking { get; set; }
        public int numberEquipmentActive { get; set; }
        public int numberEquipmentUnactive { get; set; }

        public bool isAdmin { get; set; }


        public DashboardViewModel(account currenUser)
        {
            using (var ctx = new DBContext())
            {
                if (currenUser.role_id == 1)
                {
                    numberAccount = ctx.accounts.Where(acc => acc.is_deleted == false).ToList().Count;
                    numberRoom = ctx.rooms.Where(r => r.is_deleted == false).ToList().Count;
                    numberEquipment = ctx.equipments.Where(e => e.is_deleted == false).ToList().Count;
                    numberBooking = ctx.bookings.ToList().Count;
                    numberEquipmentActive = ctx.equipments.Where(e => e.is_deleted == false && e.status == true).ToList().Count;
                    isAdmin = true;
                }
                else
                {
                    numberBooking = ctx.rooms.Where(r => r.is_deleted == false && r.leader_id == currenUser.id)
                        .SelectMany(r => r.bookings).ToList().Count;

                    numberEquipment = ctx.rooms.
                        Where(r => r.leader_id == currenUser.id && r.is_deleted == false)
                        .SelectMany(r => r.equipments)
                        .Where(equip => equip.is_deleted == false).ToList().Count;

                    numberEquipmentActive = ctx.rooms.
                        Where(r => r.leader_id == currenUser.id && r.is_deleted == false)
                        .SelectMany(r => r.equipments)
                        .Where(equip => equip.is_deleted == false && equip.status == true).ToList().Count;
                    isAdmin = false;
                }
                numberEquipmentUnactive = numberEquipment - numberEquipmentActive;
            }
        }
    }
}