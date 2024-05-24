using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Role
{
    public interface IRoleBll
    {
        List<Entities.Models.Role> GetAllRoles();
        Entities.Models.Role NewRole(string roleName);
    }
}