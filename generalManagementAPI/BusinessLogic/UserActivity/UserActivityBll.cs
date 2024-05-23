using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.AppContext;
using Entities.Models;

namespace BusinessLogic.UserActivity
{
    public class UserActivityBll : IUserActivityBll
    {
        #region Fields
        private readonly Context _context;
        #endregion
        
        #region Constructor
        public UserActivityBll()
        {
            _context = new Context();
        } 
        #endregion

        public UsersActivity GetAllUserActivity()
        {
            throw new NotImplementedException();
        }

        public UsersActivity GetUserActivity(int UserId)
        {
            throw new NotImplementedException();
        }

        public UsersActivity NewUserActivity(UsersActivity newUserActivity)
        {
            var result = _context.UsersActivities.Add(newUserActivity);
            _context.SaveChanges();

            return result.Entity;
        }
    }
}