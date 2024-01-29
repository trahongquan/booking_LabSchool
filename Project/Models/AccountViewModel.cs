using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class AccountViewModel
    {
        public account account { get; set; }
        public List<account> listAccount { get; set; }
        public List<role> listRole { get; set; }

        public AccountViewModel()
        {
            using (DBContext ctx = new DBContext())
            {
                account = new account();
                listAccount = ctx.accounts.Where(m => m.is_deleted == false).ToList();
                listRole = (from p in ctx.roles
                            where p.role_name != "ADMIN"
                            select p
                    ).ToList();
            }
        }

        public string getRoleName(int? roleId)
        {
            using (DBContext ctx = new DBContext())
            {
                var role = ctx.roles.FirstOrDefault(m => m.id == roleId);
                return role == null ? "" : role.role_name;
            }
        }
    }
}