using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.AppContext;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.User
{
    public class UserBll : IUserBll
    {
        #region Fields
        private readonly Context _context;
        #endregion

        #region Constructors
        public UserBll()
        {
            _context = new Context();
        }
        #endregion
        public Entities.Models.User DisableUser()
        {
            throw new NotImplementedException();
        }

        public List<Entities.Models.User> GetAllUsers()
        {
            var userList = _context.Users.ToList();

            return userList;
        }

        public Entities.Models.User LogInUser()
        {
            throw new NotImplementedException();
        }

        public Entities.Models.User NewUser()
        {
            throw new NotImplementedException();
        }
    }
}