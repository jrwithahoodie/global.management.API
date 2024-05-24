using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.AppContext;

namespace BusinessLogic.Role
{
    public class RoleBll : IRoleBll
    {
        #region Fields
        private readonly Context _context;
        #endregion

        #region  Constructors
        public RoleBll()
        {
            _context = new Context();
        }
        #endregion
        public List<Entities.Models.Role> GetAllRoles()
        {
            var rolesList = _context.Roles.ToList();

            return rolesList;
        }

        public Entities.Models.Role NewRole(string roleName)
        {
            var rolesListName = _context.Roles.Where(r => r.RoleName == roleName).ToList().FirstOrDefault();

            if(rolesListName != null)
                throw new Exception("Este rol ya existe");

            var newRoleObj = new Entities.Models.Role();
            newRoleObj.RoleName = roleName;

            var newUserResult = _context.Roles.Add(newRoleObj);
            _context.SaveChanges();

            return newUserResult.Entity;
        }
    }
}