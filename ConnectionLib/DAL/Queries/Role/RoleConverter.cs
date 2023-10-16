using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLib.DAL.Queries.Role
{
    public static class RoleConverter
    {
        public static Enteties.Role Convert(Enteties.Role role, UpdateRoleQuery urq)
        {
            role.Description = (!string.IsNullOrEmpty(urq.NewDescription)) ? urq.NewDescription : role.Description;
            return role;
        }
    }
}