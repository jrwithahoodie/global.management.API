using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace BusinessLogic.UserActivity
{
    public interface IUserActivityBll
    {
        Entities.Models.UsersActivity NewUserActivity(UsersActivity newUserActivity);
        Entities.Models.UsersActivity GetUserActivity(int UserId);
        Entities.Models.UsersActivity GetAllUserActivity();
    }
}