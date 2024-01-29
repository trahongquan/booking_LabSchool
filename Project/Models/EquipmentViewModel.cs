using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Project.Models
{
    public class EquipmentViewModel
    {
        private readonly int ROLE_ADMIN = 1;
        public equipment equip { get; set; }
        public List<equipment> listEquipment { get; set; }
        public bool isAdmin { get; set; } = false;
        public string status { get; set; }
        public string textSearch { get; set; } = string.Empty;
        public EquipmentViewModel(account currentUser)
        {

            using (DBContext ctx = new DBContext())
            {
                equip = new equipment();
                if (currentUser.role_id == ROLE_ADMIN)
                {
                    listEquipment = ctx.equipments.Where(item => item.is_deleted == false).ToList();
                    isAdmin = true;
                }
                else
                {
                    listEquipment = ctx.rooms.
                        Where(r => r.leader_id == currentUser.id && r.is_deleted == false)
                        .SelectMany(r => r.equipments)
                        .Where(equip => equip.is_deleted == false).ToList();
                    isAdmin = false;
                }
            }
        }
        public EquipmentViewModel(account currentUser, string text)
        {

            using (DBContext ctx = new DBContext())
            {
                equip = new equipment();
                if (currentUser.role_id == ROLE_ADMIN)
                {
                    listEquipment = ctx.equipments.Where(item => item.is_deleted == false).ToList();
                    isAdmin = true;
                }
                else
                {
                    listEquipment = ctx.rooms.
                        Where(r => r.leader_id == currentUser.id && r.is_deleted == false)
                        .SelectMany(r => r.equipments)
                        .Where(equip => equip.is_deleted == false).ToList();
                    isAdmin = false;
                }
                var l = new List<equipment>();
                listEquipment.ForEach(item =>
                {
                    if (convertToUnSign3(item.equipment_type.ToLower()).Contains(text))
                    {
                        l.Add(item);
                    }
                });
                listEquipment = l;
            }
        }

        public List<SelectListItem> listRoomSelection()
        {
            var list = new List<SelectListItem>();
            using (DBContext ctx = new DBContext())
            {

                ctx.rooms.ForEach(r =>
                {
                    list.Add(new SelectListItem { Value = r.id.ToString(), Text = r.room_name });
                });
            }
            return list;
        }

        public string getRoomName(int? room_id)
        {
            using (DBContext ctx = new DBContext())
            {
                var room = ctx.rooms.FirstOrDefault(r => r.id == room_id);
                if (room != null) return room.room_name;
            }
            return string.Empty;
        }

        // function để xóa bỏ kí tự, ví dụ:    quạt -> quat, tủ lạnh -> tu lanh
        private string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}