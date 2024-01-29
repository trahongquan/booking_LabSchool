using Microsoft.Ajax.Utilities;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Project.Areas.Admin.Controllers
{
    public class EquipmentController : Controller
    {
        private static readonly int ROLE_TEACH = 3;
        private static readonly int ROLE_LEADER = 2;
        private static readonly int ROLE_ADMIN = 1;
        // GET: Admin/Equipment
        public ActionResult Index()
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                using (DBContext ctx = new DBContext())
                {
                    var listEquip = new EquipmentViewModel(user);
                    return View(listEquip);
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        public ActionResult Search(string textSearch)
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                textSearch = convertToUnSign3(textSearch).ToLower();
                var model = new EquipmentViewModel(user, textSearch);
                return View("Index", model);
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        public ActionResult removeEquipmentFromRoom(int roomid, int equipid)
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id == ROLE_LEADER)
            {
                using (DBContext ctx = new DBContext())
                {
                    var item = ctx.equipments.FirstOrDefault(eq => eq.id == equipid && eq.room_id == roomid);
                    var room = ctx.rooms.FirstOrDefault(r => r.id == roomid);
                    if (item != null && room != null)
                    {
                        item.room_id = null;
                        item.room = null;

                        room.equipments.Remove(item);
                        ctx.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }


        public JsonResult sort(string sort)
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)
            {
                List<equipment> listEquipment;
                using (var ctx = new DBContext())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    if (user.role_id == ROLE_LEADER)
                    {
                        if (sort.Equals("type"))
                        {
                            listEquipment = ctx.equipments.Where(eq => eq.is_deleted == false && eq.room.leader_id == user.id).OrderBy(eq => eq.equipment_type).ToList();
                        }
                        else if (sort.Equals("status"))
                        {
                            listEquipment = ctx.equipments.Where(eq => eq.is_deleted == false && eq.room.leader_id == user.id).OrderByDescending(eq => eq.status).ToList();
                        }
                        else if (sort.Equals("room"))
                        {
                            listEquipment = ctx.equipments.Where(eq => eq.is_deleted == false && eq.room.leader_id == user.id).OrderBy(eq => eq.room_id).ToList();
                        }
                        else
                        {
                            listEquipment = ctx.equipments.Where(eq => eq.is_deleted == false && eq.room.leader_id == user.id).ToList();
                        }
                    }
                    else
                    {
                        if (sort.Equals("type"))
                        {
                            listEquipment = ctx.equipments.Where(eq => eq.is_deleted == false).OrderBy(eq => eq.equipment_type).ToList();
                        }
                        else if (sort.Equals("status"))
                        {
                            listEquipment = ctx.equipments.Where(eq => eq.is_deleted == false).OrderByDescending(eq => eq.status).ToList();
                        }
                        else if (sort.Equals("room"))
                        {
                            listEquipment = ctx.equipments.Where(eq => eq.is_deleted == false).OrderBy(eq => eq.room_id).ToList();
                        }
                        else
                        {
                            listEquipment = ctx.equipments.Where(eq => eq.is_deleted == false).ToList();
                        }
                    }
                }
                var listEquip = new EquipmentViewModel(user);
                var html = "";
                listEquipment.ForEach(m =>
                {
                    var stt = m.status == true ? "Còn hoạt động" : "Bị hỏng";

                    var btnEdit = $"<a href='/Admin/Equipment/edit/{m.id}' class='btn btn-success my-2 my-sm-0'>Sửa</a>";
                    var btnDelete = $"<a href='/Admin/Equipment/Delete/{m.id}' class='btn btn-danger my-2 my-sm-0' onclick = `if(!(confirm('Bạn có muốn xóa thiết bị này không?') return false;`>Xóa</a>";
                    html += "<tr>"
                    + $"<td scope=\"row\"></td>"
                    + $"<td>{m.equipment_number}</td>"
                    + $"<td>{m.equipment_type}</td>"
                    + $"<td>{listEquip.getRoomName(m.room_id)}</td>"
                    + $"<td>{m.production_year}</td>"
                    + $"<td>{m.origin}</td>"
                    + $"<td>{m.voltage}</td>"
                    + $"<td>{stt}</td>"
                    + $"<td>{btnEdit} | {btnDelete}</td>";
                });
                return Json(new { html }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Error = "Vui lòng đăng nhập" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult add()
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                var listEquip = new EquipmentViewModel(user);
                return View(listEquip);
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        [HttpPost]
        public ActionResult Add(equipment equip, string status)
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                using (DBContext dbContext = new DBContext())
                {
                    var item = dbContext.equipments.FirstOrDefault(it => it.equipment_number.ToLower().Equals(equip.equipment_number.ToLower()));
                    if (item != null)
                    {
                        if (item.is_deleted == true)
                        {
                            item.equipment_type = equip.equipment_type;
                            item.production_year = equip.production_year;
                            item.voltage = equip.voltage;
                            item.room_id = equip.room_id;
                            item.origin = equip.origin;
                            item.status = (status == "D");
                            item.is_deleted = false;
                            dbContext.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        ViewBag.Message = "Số hiệu thiết bị đã tồn tại";
                        var listEquip = new EquipmentViewModel(user);
                        return View("add", listEquip);
                    }
                    equip.status = (status == "D");
                    dbContext.equipments.Add(equip);
                    dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        public ActionResult edit(int id)
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                var listEquip = new EquipmentViewModel(user);
                using (DBContext ctx = new DBContext())
                {
                    var item = ctx.equipments.FirstOrDefault(x => x.id == id);
                    if (item != null)
                    {
                        listEquip.equip = item;
                        return View(listEquip);
                    }
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        [HttpPost]
        public ActionResult Edit(equipment equip, string status)
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                using (DBContext ctx = new DBContext())
                {
                    var item = ctx.equipments.FirstOrDefault(m => m.id == equip.id);
                    if (item != null)
                    {
                        item.equipment_type = equip.equipment_type;
                        item.production_year = equip.production_year;
                        item.voltage = equip.voltage;
                        item.room_id = equip.room_id;
                        item.origin = equip.origin;
                        item.status = (status == "D");
                        ctx.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        public ActionResult Delete(int id)
        {
            var user = Session["user"] as account;
            if (user != null && user.role_id != ROLE_TEACH)      // role_id = 3 (Teacher)
            {
                using (DBContext ctx = new DBContext())
                {
                    var item = ctx.equipments.FirstOrDefault(m => m.id == id);
                    if (item != null)
                    {
                        item.is_deleted = true;
                        ctx.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Login", new { area = "" });
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