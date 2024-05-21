using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace BusinessLogic.User
{
    public interface IUserBll
    {
        Entities.Models.User NewUser ();
        List<Entities.Models.User> GetAllUsers();
        Entities.Models.User DisableUser();
        Entities.Models.User LogInUser();
    }
}