using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Models
{
    public class RoomViewModel
    {
        private readonly int LEADER_CLASS = 2;
        private readonly int ADMIN = 1;
        public room room { get; set; }
        public List<room> roomList { get; set; }
        public List<account> accountList { get; set; }
        public string leaderClass { get; set; }
        public room roomLead { get; set; }
        public bool isAdmin { get; set; }
        public RoomViewModel(account acc)
        {
            room = new room();
            using (DBContext ctx = new DBContext())
            {
                if (acc.role_id == ADMIN)
                {
                    roomList = ctx.rooms.Where(r => r.is_deleted == false).ToList();
                    accountList = ctx.accounts.Where(a => a.role_id == LEADER_CLASS && a.is_deleted == false
                                        && !ctx.rooms.Any(r => r.leader_id == a.id && room.is_deleted == false)).ToList();
                    isAdmin = true;
                }
                else if (acc.role_id == LEADER_CLASS)
                {
                    isAdmin = false;
                    leaderClass = acc.fullname;
                    roomLead = ctx.rooms.FirstOrDefault(m => m.leader_id == acc.id && m.is_deleted == false);
                }
            }

        }

        public string getLeaderClass(int? accountId)
        {
            using (DBContext ctx = new DBContext())
            {
                var acc = ctx.accounts.FirstOrDefault(m => m.id == accountId && m.role_id == LEADER_CLASS);
                return acc == null ? "" : acc.fullname;
            }
        }
    }
}